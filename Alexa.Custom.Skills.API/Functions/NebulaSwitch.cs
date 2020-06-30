using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using Alexa.NET.Response;
using Alexa.NET.Request.Type;
using Alexa.NET;
using Alexa.NET.Request;
using System.Configuration;
using System.Data.SqlClient;
using Alexa.Custom.Skills.API.Logging;

namespace Alexa.Custom.Skills.API.Functions
{
    public static class NebulaSwitch
    {
        [FunctionName("nebulaswitchping")]
        public static async Task<IActionResult> RunPing([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {            
            return new OkObjectResult($"Hi from nebula switch");

        }
        [FunctionName("nebulaswitch")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string _id = Guid.NewGuid().ToString();
            AppInsightHelper.Instance.AppInsightInit(new AppInsightPayload
            {
                _operation = "nebulaswitch",
                _type = AppInsightLanguage.AppInsightTrace,
                _payload = "Alexa call started",
                _correlationId = _id.ToString()
            });
            
            SkillResponse response = null;
            try
            {
                log.LogInformation("In Alexa Switch Skill");
                string json = await req.ReadAsStringAsync();
                //json = data;
                log.LogInformation(json);

                if (!string.IsNullOrEmpty(json))
                {
                    AppInsightHelper.Instance.AppInsightInit(new AppInsightPayload
                    {
                        _operation = "nebulaswitch",
                        _type = AppInsightLanguage.AppInsightTrace,
                        _payload = "Welcome message",
                        _correlationId = _id.ToString()
                    });
                    var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(json);
                    var requestType = skillRequest.GetRequestType();

                    if (requestType != null)
                    {
                        if (requestType == typeof(LaunchRequest))
                        {
                            response = ResponseBuilder.Tell("Welcome to Nebula Switch. Please say display on or off  or tv switch on or off");
                            response.Response.ShouldEndSession = false;
                            return new OkObjectResult(response);
                        }
                    }

                    if (skillRequest != null)
                    {
                        log.LogInformation($"In Intent");
                        if (((Alexa.NET.Request.Type.IntentRequest)skillRequest.Request).Intent.Name.ToString().ToUpper() == "displayswitch".ToUpper())
                        {
                            AppInsightHelper.Instance.AppInsightInit(new AppInsightPayload
                            {
                                _operation = "nebulaswitch",
                                _type = AppInsightLanguage.AppInsightTrace,
                                _payload = "In dislayswitch",
                                _correlationId = _id.ToString()
                            });
                            log.LogInformation($"Intent Name is displayswitch ");
                            System.Collections.Generic.Dictionary<string, Slot> _slots = ((Alexa.NET.Request.Type.IntentRequest)skillRequest.Request).Intent.Slots;
                            string stateValue = _slots["item"].Value;

                            #region DBCall
                            AppInsightHelper.Instance.AppInsightInit(new AppInsightPayload
                            {
                                _operation = "nebulaswitch",
                                _type = AppInsightLanguage.AppInsightTrace,
                                _payload = "Calling DB to update",
                                _correlationId = _id.ToString()
                            });
                            string commandText = string.Empty;
                            bool dbresponse = false;
                            if (stateValue.ToUpper().Trim().Equals("ON".ToUpper().Trim()))
                                commandText = $"Update [TRX].[tblNebulaSwitch] SET [state] =1 WHERE ID=1;";
                            else
                                commandText = $"Update [TRX].[tblNebulaSwitch] SET [state] =0 WHERE ID=1;";
                            dbresponse = await DBUpdateSwitch(log, commandText, _id.ToString());

                            #endregion

                            log.LogInformation($"State value {stateValue}");
                            response = ResponseBuilder.Tell($"Switch state set to {stateValue}. Device will be update with new state in 30 seconds");
                            response.Response.ShouldEndSession = false;
                            AppInsightHelper.Instance.AppInsightInit(new AppInsightPayload
                            {
                                _operation = "nebulaswitch",
                                _type = AppInsightLanguage.AppInsightTrace,
                                _payload = "Update complete",
                                _correlationId = _id.ToString()
                            });
                            return new OkObjectResult(response);
                        }
                        else
                        {
                            AppInsightHelper.Instance.AppInsightInit(new AppInsightPayload
                            {
                                _operation = "nebulaswitch",
                                _type = AppInsightLanguage.AppInsightTrace,
                                _payload = "Intent missing",
                                _correlationId = _id.ToString()
                            });
                            log.LogInformation($"In else Intent");
                            response = ResponseBuilder.Tell("Intent missing");
                            response.Response.ShouldEndSession = false;
                            return new OkObjectResult(response);
                        }
                    }
                    else
                    {
                        AppInsightHelper.Instance.AppInsightInit(new AppInsightPayload
                        {
                            _operation = "nebulaswitch",
                            _type = AppInsightLanguage.AppInsightTrace,
                            _payload = "Skill request null",
                            _correlationId = _id.ToString()
                        });
                        log.LogInformation($"In else skillRequest");
                        response = ResponseBuilder.Tell("skillRequest is null");
                        response.Response.ShouldEndSession = false;
                        return new OkObjectResult(response);
                    }
                }
                else
                {
                    AppInsightHelper.Instance.AppInsightInit(new AppInsightPayload
                    {
                        _operation = "nebulaswitch",
                        _type = AppInsightLanguage.AppInsightTrace,
                        _payload = "Empty Json",
                        _correlationId = _id.ToString()
                    });
                    log.LogInformation($"In else json");
                    response = ResponseBuilder.Tell("No request available");
                    response.Response.ShouldEndSession = false;
                    return new OkObjectResult(response);
                }

            }
            catch (Exception ex)
            {
                AppInsightHelper.Instance.AppInsightInit(new AppInsightPayload
                {
                    _operation = "nebulaswitch",
                    _type = AppInsightLanguage.AppInsightException,
                    _payload = "Error",
                    _correlationId = _id.ToString(),
                    _ex = ex

                });
                log.LogInformation($"In exception");
                response = ResponseBuilder.Tell($"Exception : {ex.Message}");
                response.Response.ShouldEndSession = false;
                return new OkObjectResult(response);

            }
        }

        [FunctionName("getnebulaswitchstatus")]
        public static async Task<int> GetNebulaSwitchStatus([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ILogger log)
        {
            int state = -1;
            try
            {
                log.LogInformation("In Switch Get API");
                #region DBCall
                string commandText = $"Select state from [TRX].[tblNebulaSwitch] WHERE ID=1;";
                try
                {
                    var sqlConnection = Environment.GetEnvironmentVariable("DBConnect", EnvironmentVariableTarget.Process);

                    using (SqlConnection connection = new SqlConnection(sqlConnection))
                    {
                        try
                        {
                            SqlCommand command = new SqlCommand(commandText, connection);
                            connection.Open();
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Console.WriteLine($"Value {reader[0]}");
                                    if (Convert.ToBoolean(reader[0])) state = 1;
                                    else state = 0;

                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            log.LogInformation("Error ({0}): {1}", ex.Number, ex.Message);
                            state = -1;
                        }
                        catch (InvalidOperationException ex)
                        {
                            log.LogInformation("Error: {0}", ex.Message);
                            state = -1;
                        }
                        catch (Exception ex)
                        {
                            log.LogInformation("Error: {0}", ex.Message);
                            state = -1;
                        }
                    }

                }
                catch (Exception ex)
                {

                    throw;
                }


                #endregion
                return state;
            }
            catch (Exception)
            {

                throw;
            }

        }
        private static async Task<bool> DBUpdateSwitch(ILogger log, string commandText, string _id)
        {
            try
            {
                AppInsightHelper.Instance.AppInsightInit(new AppInsightPayload
                {
                    _operation = "DBUpdateSwitch",
                    _type = AppInsightLanguage.AppInsightTrace,
                    _payload = $"In DB method, command :{commandText}",
                    _correlationId = _id.ToString()
                });
                var sqlConnection = Environment.GetEnvironmentVariable("DBConnect", EnvironmentVariableTarget.Process);

                using (SqlConnection connection = new SqlConnection(sqlConnection))
                {
                    try
                    {
                        int count = 0;
                        SqlCommand command = new SqlCommand(commandText, connection);
                        connection.Open();

                        IAsyncResult result =
                            command.BeginExecuteNonQuery();
                        while (!result.IsCompleted)
                        {
                            log.LogInformation("Waiting ({0})", count++);

                            System.Threading.Thread.Sleep(100);
                        }
                        log.LogInformation("Command complete. Affected {0} rows.",
                        command.EndExecuteNonQuery(result));
                        AppInsightHelper.Instance.AppInsightInit(new AppInsightPayload
                        {
                            _operation = "DBUpdateSwitch",
                            _type = AppInsightLanguage.AppInsightTrace,
                            _payload = $"DB request completed",
                            _correlationId = _id.ToString()
                        });
                        return true;
                    }
                    catch (SqlException ex)
                    {
                        log.LogInformation("Error ({0}): {1}", ex.Number, ex.Message);
                        AppInsightHelper.Instance.AppInsightInit(new AppInsightPayload
                        {
                            _operation = "DBUpdateSwitch",
                            _type = AppInsightLanguage.AppInsightException,
                            _payload = $"Error",
                            _correlationId = _id.ToString(),
                            _ex = ex
                        });
                        return false;
                    }
                    catch (InvalidOperationException ex)
                    {
                        log.LogInformation("Error: {0}", ex.Message);
                        AppInsightHelper.Instance.AppInsightInit(new AppInsightPayload
                        {
                            _operation = "DBUpdateSwitch",
                            _type = AppInsightLanguage.AppInsightException,
                            _payload = $"Error",
                            _correlationId = _id.ToString(),
                            _ex = ex
                        });
                        return false;
                    }
                    catch (Exception ex)
                    {
                        log.LogInformation("Error: {0}", ex.Message);
                        AppInsightHelper.Instance.AppInsightInit(new AppInsightPayload
                        {
                            _operation = "DBUpdateSwitch",
                            _type = AppInsightLanguage.AppInsightException,
                            _payload = $"Error",
                            _correlationId = _id.ToString(),
                            _ex = ex
                        });
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                AppInsightHelper.Instance.AppInsightInit(new AppInsightPayload
                {
                    _operation = "DBUpdateSwitch",
                    _type = AppInsightLanguage.AppInsightException,
                    _payload = $"Error",
                    _correlationId = _id.ToString(),
                    _ex = ex
                });
                throw;
            }
        }
    }

}
