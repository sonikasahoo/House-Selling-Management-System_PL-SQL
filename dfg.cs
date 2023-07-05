using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BloodBankManagementSystem.Models;

namespace BloodBankManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessRequestBloodAndCheck_Controller : ControllerBase
    {
        private readonly string connectionString = "Data Source=LTIN237427;Initial Catalog=DD_design;Integrated Security=True";
        /*public BusinessRequestBloodAndCheck_Controller(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection"); ;
        }*/

        [HttpPost]
        public IActionResult MakeRequest(RequestBlood request)
        {
            try
            {
                using(var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var query = @"INSERT INTO requestblood(RequestorId, PatientName, Required_Blood_Group, City, Doctors_Name, Hospital_name_Address, Blood_Required_Date, Contact_Name, Contact_Number, Contact_Email_id, Message)
                                  VALUES
                                  (@RequestorId, @Patient_Name, @Requested_Blood_Group, @City, @DoctorName, @Hospital_Name_Address, @Blood_required_Date, @Contact_Name, @Contact_Number, @Contact_Email_Id, @Message)";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@RequestorId", request.RequestorId);
                        command.Parameters.AddWithValue("@Patient_Name", request.Patient_Name);
                        command.Parameters.AddWithValue("@Requested_Blood_Group", request.Requested_Blood_Group);
                        command.Parameters.AddWithValue("@City", request.City);
                        command.Parameters.AddWithValue("@DoctorName", request.DoctorName);
                        command.Parameters.AddWithValue("@Hospital_Name_Address", request.Hospital_Name_Address);
                        command.Parameters.AddWithValue("@Blood_required_Date", request.Blood_required_Date);
                        command.Parameters.AddWithValue("@Contact_Name", request.Contact_Name);
                        command.Parameters.AddWithValue("@Contact_Number", request.Contact_Number);
                        command.Parameters.AddWithValue("@Contact_Email_Id", request.Contact_Email_Id);
                        command.Parameters.AddWithValue("@Message", request.Message);

                        command.ExecuteNonQuery();
                    }
                    return Ok("Blood Request successfully submitted.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error occurred while making the blood request" + ex.Message);
            }
        }

        [HttpGet("{requestorId}")]
        public IActionResult GetRequestorStatus(string requestorId)
        {
            try
            {
                using(var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT * FROM requeststatus WHERE RequestorId = @RequestorId";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@RequestorId", requestorId);
                        using (var reader = command.ExecuteReader())
                        {
                            if(reader.HasRows)
                            {
                                var result = new List<RequestStatus>();
                                while(reader.Read())
                                {
                                    var requestStatus = new RequestStatus
                                    {
                                        RequestorId = reader.GetString("RequestorId"),
                                        PatientId = reader.GetString("PatientId"),
                                        Times_Of_The_Day = reader.GetDateTime("Times_Of_The_Day"),
                                        Blood_Glucose_Level = reader.GetString("Blood_Glucose_Level"),
                                        Notes = reader.GetString("Notes")
                                    };
                                    result.Add(requestStatus);
                                }
                                return Ok(result);
                            }
                        }
                    }
                    return NotFound("Requestor Details Not Found");

                }
            }
            catch(Exception ex)
            {
                return BadRequest("Error Occurred while retrieving the requestor details : " + ex.Message);
            }
        }

    }
}
