using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;

namespace CampusManagement.Controllers
{
    public class StudentServicesController : ApiController
    {
        public List<StudentData> Get()
        {
            DbManagement db_data = new DbManagement();
            return db_data.GetAllStudentsData();
        }
        public HttpResponseMessage GetbyID(int id)
        {
            DbManagement db_data = new DbManagement();
            StudentData student = db_data.GetStudentsDatabyID(id);
            if (student != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, student);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Student not found");
            }
        }
        public HttpResponseMessage Post([FromBody] StudentData student)
        {
            DbManagement db_data = new DbManagement();
            int rows_affected = db_data.AddNewStudent(student);
            if (rows_affected != -2)
            {
                return Request.CreateResponse(HttpStatusCode.OK, rows_affected.ToString() + " row(s) affected");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, "Error Adding student student");
            }
        }

    }

    public class StudentData
    {
        public int ID;
        public string FirstName;
        public string LastName;
        public double GPA;

        public StudentData(int ID, string first, string last, double GPA)
        {
            this.ID = ID;
            this.FirstName = first;
            this.LastName = last;
            this.GPA = GPA;
        }
        public void Set(int ID, string first, string last, double GPA)
        {
            this.ID = ID;
            this.FirstName = first;
            this.LastName = last;
            this.GPA = GPA;
        }

    }

    public class DbManagement
    {
        object connectionString;
        public DbManagement()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["testdb"];
        }
        public List<StudentData> GetAllStudentsData()
        {
            try
            {
                List<StudentData> student_array;
                student_array = new List<StudentData>();
                using (SqlConnection connection = new SqlConnection(connectionString.ToString()))
                {
                    connection.Open();

                    String sql = "EXEC storProcedure";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                student_array.Add(new StudentData(Convert.ToInt32(reader["ID"]), reader["FirstName"].ToString(), reader["LastName"].ToString(), Convert.ToDouble(reader["GPA"])));
                            }
                            return student_array;
                        }
                    }
                }
            }

            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
        public StudentData GetStudentsDatabyID(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString.ToString()))
                {
                    connection.Open();
                    String sql_query = "SELECT * FROM student where id=" + id.ToString();
                    using (SqlCommand command = new SqlCommand(sql_query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                return (new StudentData(Convert.ToInt32(reader["ID"]), reader["FirstName"].ToString(), reader["LastName"].ToString(), Convert.ToDouble(reader["GPA"])));
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }

            catch (SqlException e)
            {
                return null;
            }
        }
        public int AddNewStudent(StudentData student)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString.ToString()))
                {
                    connection.Open();
                    String sql_query = "Insert into student Values (" + (student.ID).ToString() + ",'" + student.FirstName + "', '" + student.LastName + "', " + student.GPA + ")";
                    using (SqlCommand command = new SqlCommand(sql_query, connection))
                    {
                        int rows_affected = command.ExecuteNonQuery();
                        return rows_affected;
                    }
                }
            }
            catch (SqlException sqle)
            {
                return -2;
                //to start continuous build
            }
        }
    }
}