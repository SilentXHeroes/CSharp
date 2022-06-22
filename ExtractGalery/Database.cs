using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using FileWriter;
using System.Windows.Forms;

namespace GaleryParseContent
{
    public class Database
    {
        protected SqlConnection db;
        private SqlDataReader commandResult;
        private SqlCommand command;

        private int sqlCommandCounter = 0;

        private string _typeStatement = "";
        private string _table = "";
        private List<string> _select = new List<string>();
        private List<string> _where = new List<string>();
        private List<string> _join = new List<string>();
        private List<string> _orderBy = new List<string>();
        private List<string> _columns = new List<string>();
        private List<string> _values = new List<string>();
        private List<string> _setters = new List<string>();
        private bool enableLogs = false;

        public Database()
        {
            //db = new DBHandler("Georges");
            db = new SqlConnection("Data Source=LOCALHOST;Initial Catalog=Georges;User ID=cepi;Password=");
            db.Open();
        }
        public Database set(string column, dynamic value)
        {
            return set(column, value, true);
        }
        public Database set(string column, dynamic value, bool parse)
        {
            if (parse) value = cleanString(value);
            _setters.Add($"{column}={value}");
            return this;
        }

        public void EnableWriteLogs()
        {
            enableLogs = true;
        }

        public void DisableWriteLogs()
        {
            enableLogs = true;
        }

        public int insert(string table)
        {
            return InsertStatement(table);
        }

        public int insertRow(string table, Dictionary<string, dynamic> entry)
        {
            return insert(table, new List<Dictionary<string, dynamic>> { entry });
        }

        public int insert(string table, List<Dictionary<string, dynamic>> entries)
        {
            int loop = 0;

            foreach(Dictionary<string, dynamic> entry in entries)
            {
                if(loop == 0)
                {
                    foreach(string column in entry.Keys) _columns.Add(column);
                }
                else if(entry.Keys.Count != _columns.Count)
                {
                    throw new InvalidProgramException("Le nombre de colonnes ne correspondent pas !!");
                }

                List<dynamic> values = new List<dynamic>();
                foreach(dynamic value in entry.Values)
                {
                    values.Add(cleanString(value));
                }

                _values.Add($"({ String.Join(",", values) })");
                loop++;
            }

            return InsertStatement(table);
        }

        public void update(string table)
        {
            _table = table;
            _typeStatement = "UPDATE";
            ExecuteStatement();
        }

        public Database select(string str)
        {
            _typeStatement = "SELECT";
            _select.Add(str);
            return this;
        }

        public Database from(string str)
        {
            _table = str;
            return this;
        }

        public Database where(string column, dynamic value)
        {
            if (
                column.IndexOf('<') == -1 &&
                column.IndexOf('>') == -1 &&
                column.IndexOf('=') == -1 &&
                column.IndexOf("<>") == -1
            )
            {
                column += '=';
            }
            _where.Add(column + cleanString(value));
            return this;
        }

        public Database join(string table, string cond = null, string side = "left")
        {
            _join.Add($"{side.ToUpper()} JOIN {table} ON {cond}");
            return this;
        }

        public Database order_by(string order)
        {
            _orderBy.Add(order);
            return this;
        }

        private Dictionary<string, dynamic> _fetch(string sql = "")
        {
            _typeStatement = "SELECT";
            ExecuteStatement(sql);
            commandResult.Read();
            Dictionary<string, dynamic> item = getCurrentItem();
            close();

            return item;
        }

        public Dictionary<string, dynamic> fetch()
        {
            return _fetch();
        }

        public List<Dictionary<string, dynamic>> fetch_array()
        {
            List<Dictionary<string, dynamic>> result = new List<Dictionary<string, dynamic>>();

            ExecuteStatement();

            if (commandResult.IsClosed) return result;

            while (commandResult.Read())
            {
                result.Add(getCurrentItem());
            }

            close();
            return result;
        }

        private void close()
        {
            commandResult.Close();
            command.Dispose();
        }

        private void newCommand(string sql)
        {
            sqlCommandCounter++;
            command = new SqlCommand(sql, db);
        }

        private Dictionary<string, dynamic> getCurrentItem()
        {
            Dictionary<string, dynamic> item = new Dictionary<string, dynamic>();

            if (commandResult.HasRows)
            {
                for (var i = 0; i < commandResult.FieldCount; i++)
                {
                    dynamic key = commandResult.GetName(i);
                    dynamic value = commandResult.GetValue(i);
                    //if (enableLogs) WriteLog.WriteLine(key +" => "+ value);
                    item.Add(key, value);
                }
            }

            return item;
        }

        private int InsertStatement(string table)
        {
            _table = table;
            _typeStatement = "INSERT";
            return ExecuteStatement();
        }

        public dynamic ExecuteStatement(string sql = "")
        {
            if (sql == "")
            {
                string where = String.Join(" AND ", _where);
                string join = String.Join(" ", _join);
                string orderBy = String.Join(" ", _orderBy);

                if (where != "") where = " WHERE " + where;
                if (join != "") join = " " + join;
                if (orderBy != "") orderBy = " ORDER BY " + orderBy;

                if (_typeStatement == "INSERT")
                {
                    sql = $"INSERT INTO {_table} ({String.Join(",", _columns)}) VALUES { String.Join(",", _values)}";
                }
                else if(_typeStatement == "SELECT")
                {
                    sql = $"SELECT {String.Join(",", _select)} FROM {_table}" + join + where + orderBy;
                }
                else if(_typeStatement == "UPDATE")
                {
                    sql = $"UPDATE {_table} SET {String.Join(",", _setters)}" + where;
                }
            }

            if (enableLogs) WriteLog.WriteLine("SQL: " + sql);

            newCommand(sql);

            try
            {
                if (_typeStatement == "SELECT")
                {
                    commandResult = command.ExecuteReader();
                }
                else
                {
                    command.ExecuteNonQuery();
                }
            }
            catch(Exception e)
            {
                if (enableLogs) WriteLog.WriteLine("SQL ERROR - Exception caught: " + e);
                MessageBox.Show("Erreur SQL: " + e, "Une erreur est survenue", MessageBoxButtons.OK);
            }

            _typeStatement = "";
            _table = "";
            _select.Clear();
            _where.Clear();
            _join.Clear();
            _columns.Clear();
            _values.Clear();
            _setters.Clear();
            _orderBy.Clear();

            if (sql.IndexOf("INSERT") > -1) return fetchNewID();
            if (sql.IndexOf("SELECT") > -1) return this;

            return true;
        }

        private dynamic cleanString(dynamic value)
        {
            if(value is null)
            {
                value = "NULL";
            }
            else if (value is string || value is char)
            {
                if (value is string)
                {
                    // Remplacement des << \ >>
                    value = value.Replace(@"\", @"\\");
                    // Remplacement des << ' >>
                    value = value.Replace("'", "''");
                }

                value = $"'{value}'";
            }

            return value;
        }

        private int fetchNewID()
        {
            return _fetch("SELECT CAST(SCOPE_IDENTITY() AS INT) as id")["id"];
        }
    }
}
