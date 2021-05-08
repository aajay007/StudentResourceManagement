using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SRManagementApplication.Models;
using SRManagementApplication.Service;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SRManagementApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentResourceController : ControllerBase
    {
        MappingContext db;
        private IConfiguration Configuration;

        public StudentResourceController(MappingContext _db, IConfiguration _configuration)
        {
            db = _db;
            Configuration = _configuration;
        }


        //----Student-----
        //fetching student table details.
        [HttpGet("/api/Student")]
        public async Task<List<Student>> GetStudentDetails()
        {
            return await db.Student.ToListAsync();
        }

        //fetching student details with student Id.
        [HttpGet("/api/Student/{studentId}")]
        public async Task<Student> GetStudentDetailsById(int studentId)
        {
            if (db != null)
            {
                return await (from p in db.Student
                              where p.StudentId == studentId
                              select new Student 
                              {
                                  StudentId = p.StudentId,
                                  StudentName = p.StudentName,
                                  StudentAge = p.StudentAge
                              }).FirstOrDefaultAsync();
            }

            return null;
        }

        //Student table insertion.
        [HttpPost("/api/Student")]
        public async Task<int> AddStudent(Student studentDetails)
        {
            
                await db.Student.AddAsync(studentDetails);
                await db.SaveChangesAsync();

                return studentDetails.StudentId;
        }

        //Student table updation.
        [HttpPut("/api/Student")]
        public async Task PutStudent(Student studentDeatils)
        {
            if (db != null)
            {
                Student nUser = db.Student.Single(u => u.StudentId == studentDeatils.StudentId);
                nUser.StudentId = studentDeatils.StudentId;
                nUser.StudentName = studentDeatils.StudentName;
                nUser.StudentAge = studentDeatils.StudentAge;
                
                await db.SaveChangesAsync();
            }

        }

        // student table deletetion with student  Id.
        [HttpDelete("/api/Student/{studentId}")]
        public async Task<IActionResult> DeleteStudent(int studentId)
        {
            if (db != null)
            { 
                var row = await db.Student.FirstOrDefaultAsync(x => x.StudentId == studentId);

                if (row != null)
                {

                    db.Student.Remove(row);
                    await db.SaveChangesAsync();
                }
                return Ok("deletion done");
            }

            return Ok("table alredy empty");
        }


        //-----------------Resource-------------

        //fetching Resource table details
        [HttpGet("/api/Resource")]
        public async Task<List<Resource>> GetResourceDetails()
        {
            return await db.Resource.ToListAsync();
        }

        //fetching Resource table details with resourceId.
        [HttpGet("/api/Resource/{ResourceId}")]
        public async Task<Resource> GetResourceDetailsById(int resourceId)
        {
            if (db != null)
            {
                return await (from p in db.Resource
                              where p.ResourceId == resourceId
                              select new Resource
                              {
                                  ResourceId = p.ResourceId,
                                  ResourceName = p.ResourceName,
                                  ResourceTimePeriod = p.ResourceTimePeriod
                              }).FirstOrDefaultAsync();
            }

            return null;
        }

        //Resource table insertion
        [HttpPost("/api/Resource")]
        public async Task<int> AddResource(Resource resourceDetails)
        {
            await db.Resource.AddAsync(resourceDetails);
            await db.SaveChangesAsync();

            return resourceDetails.ResourceId;
        }

        //Resource table updation with ResourceId.
        [HttpPut("/api/Resource")]
        public async Task<IActionResult> PutResource(Resource resourceDeatils)
        {
            if (db != null)
            {

                Resource nUser = db.Resource.Single(u => u.ResourceId == resourceDeatils.ResourceId);
                nUser.ResourceId = resourceDeatils.ResourceId;
                if (resourceDeatils.ResourceName != null)
                {
                    nUser.ResourceName = resourceDeatils.ResourceName;
                }
                if (resourceDeatils.ResourceTimePeriod != null)
                {
                    nUser.ResourceTimePeriod = resourceDeatils.ResourceTimePeriod;
                }
               
                await db.SaveChangesAsync();
                return Ok("updation done");
            }
            return Ok("table is empty");
        }


        //Resource table deletion.
        [HttpDelete("/api/Resource/{resourceId}")]
        public async Task<IActionResult> DeleteResource(int resourceId)
        {
            
            if (db != null)
            {
                var row = await db.Resource.FirstOrDefaultAsync(x => x.ResourceId == resourceId);

                if (row != null)
                {
                    db.Resource.Remove(row);
                    await db.SaveChangesAsync();
                }
                return Ok("deletion completed");
            }

            return Ok("database table is already empty");
        }


        //-----------------Student Resource management relation mappping-----------

        [HttpPost("/api/Mapping")]
        public async Task<IActionResult> PostMethod()
        {
            var students = await db.Student.ToListAsync();
            var resources = await db.Resource.ToListAsync();

            int studentsLength = students.Count;
            int resourcesLength = resources.Count;

            if (studentsLength == 0 || resourcesLength == 0)
            {
                return Ok("Mapping not possible");
            }

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("StudentId", typeof(int)));
            dt.Columns.Add(new DataColumn("ResourceId", typeof(int)));
            dt.Columns.Add(new DataColumn("ResourceTimePeriod", typeof(string)));

            for (int i = 0; i < studentsLength; i++)
            {
                for (int j = 0; j < resourcesLength; j++)
                {

                    if (i % 2 == 0 && j% 3 == 0)
                    {
                        continue;
                    }
                    var student = await db.Student.ToListAsync();
                    var resource = await db.Resource.ToListAsync();
                    int studentId = student[i].StudentId;
                    int resourceId = resource[j].ResourceId;
                    string resourceTimePeriod = resource[j].ResourceTimePeriod;
                    dt.Rows.Add(studentId, resourceId, resourceTimePeriod);

                }
                
            }
            string connString = this.Configuration.GetConnectionString("DevConnection");
            using (SqlConnection sqlconn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand("usp_InsertStudentResourceMapping", sqlconn))
                {
                    sqlconn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@tableproducts", dt);
                    cmd.ExecuteNonQuery();
                    sqlconn.Close();
                }
            }
            return Ok("Mapping Done");
        }

    }
}
