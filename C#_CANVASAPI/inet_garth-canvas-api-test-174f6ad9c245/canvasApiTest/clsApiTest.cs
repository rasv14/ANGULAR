using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using NLog;

using canvasApiLib.API;

namespace canvasApiTest
{
	class clsApiTest
	{
		////NLog
		private static Logger _logger = LogManager.GetLogger(typeof(clsApiTest).FullName);

		private string apiFile = ".\api.config";
		private clsApiConfig apiConfig = null;
		private bool _executeAipCalls = true;

		public bool working = true;

		private enum LOG_LEVEL
		{
			DEBUG = 0,
			INFO,
			WARN,
			ERROR,
			EXCEPTION
		}

		public clsApiTest()
		{

		}

		public clsApiTest(string[] args)
		{
			parseArgs(args);
			_logger.Debug("args are parsed");
			apiConfig = new clsApiConfig();
		}

		private void parseArgs(string[] args)
		{

			if (args == null || args.GetUpperBound(0) < 0)
			{
				printArgs();
			}
			else
			{
				for (int idx = 0; idx <= args.GetUpperBound(0); idx++)
				{
					string[] param = args[idx].Split(new char[] { '=', ' ' });
					switch (param[0])
					{
						case "-f":
						case "/f":
							{
								apiFile = param[1];
								Console.WriteLine("Config file param: " + apiFile);
							}
							break;

						case "-c":
						case "/c":
							{
								createSampleConfigFile();
								_executeAipCalls = false;
							}
							break;

						case "-?":
						case "/?":
						default:
							{
								printArgs();
								_executeAipCalls = false;
							}
							break;
					}
				}
			}
		}

		private void printArgs()
		{
			Console.WriteLine("apCanvasApiTest : ");
			Console.WriteLine("   -?   will print the command line options available");
			Console.WriteLine("   -f=<config file>  defines the file containing the API calls and parameters to test");
			Console.WriteLine("   -c   write a sample config file");
		}

		private void createSampleConfigFile()
		{
			apiConfig = new clsApiConfig();
			apiConfig.accessToken = Guid.NewGuid().ToString();
			apiConfig.apiUrl = "https://mysite.instructure.com";
			List<string> vals = new List<string>();
			vals.Add("getCourseEnrollments|canvasCourseId,2550");
			apiConfig.apiCalls = vals.ToArray();
			saveConfig();
		}

		public void loadConfig()
		{
			string json = File.ReadAllText(@apiFile);
			if (!string.IsNullOrEmpty(json))
			{
				try
				{
					apiConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<clsApiConfig>(json);
				}
				catch(Exception err)
				{
					string msg = err.Message;
				}
			}
		}

		public void saveConfig()
		{
			string json = Newtonsoft.Json.JsonConvert.SerializeObject(apiConfig);
			File.WriteAllText(@apiFile, json);
			Console.WriteLine("Sample file here: " + @apiFile);
		}

		/// <summary>
		/// Write a message to the log file, and if asked to the console window
		/// Prevents us from having to write two print statements all over the place
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="level"></param>
		/// <param name="console"></param>
		/// <param name="err"></param>
		private void logMessage(string msg, LOG_LEVEL level, bool console, Exception err = null)
		{
			switch (level)
			{
				case LOG_LEVEL.DEBUG:
					{
						_logger.Debug(msg);
					}
					break;

				case LOG_LEVEL.INFO:
					{
						_logger.Info(msg);
					}
					break;

				case LOG_LEVEL.WARN:
					{
						_logger.Warn(msg);
					}
					break;

				case LOG_LEVEL.ERROR:
					{
						_logger.Error(msg);
					}
					break;

				case LOG_LEVEL.EXCEPTION:
					{
						_logger.Error(err, msg);
					}
					break;

				default:
					{
						_logger.Debug(msg);
					}
					break;
			}
			if (console)
				Console.WriteLine(msg);
		}

		public async Task<string> execute()
		{
			if (_executeAipCalls)
			{
				logMessage("Loading test config...", LOG_LEVEL.DEBUG, true);
				loadConfig();
				if (apiConfig != null && apiConfig.apiCalls.GetUpperBound(0) >= 0)
				{
					for (int idx = 0; idx <= apiConfig.apiCalls.GetUpperBound(0); idx++)
					{
						Dictionary<string, string> parameters = new Dictionary<string, string>();
						string[] api = apiConfig.apiCalls[idx].Split(new char[] { '|' });
						logMessage("Working on API call: " + api[0], LOG_LEVEL.DEBUG, true);
						switch (api[0].ToUpper())
						{
							#region ANALYTICS API CALLS
							case "GETCOURSELEVELPARTICIPATIONDATA":
								#region
								{
									int canvasCourseId = 0;

									for (int i = 1; i <= api.GetUpperBound(0); i++)
									{
										string[] vals = api[i].Split(new char[] { ',' });
										switch (vals[0].ToUpper())
										{
											case "CANVASCOURSEID":
												{
													int.TryParse(vals[1], out canvasCourseId);
												}
												break;

											default:
												{
													logMessage("--> UNUSED VARIABLE: " + vals[0] + ":" + vals[1], LOG_LEVEL.DEBUG, true);
												}
												break;
										}
									}
									try
									{
										dynamic data = await clsAnalyticsApi.getCourseLevelParticipationData(apiConfig.accessToken, apiConfig.apiUrl, canvasCourseId);
										string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
										logMessage("[" + api[0] + "] Received results: SUCCESS", LOG_LEVEL.DEBUG, true);
									}
									catch (Exception err)
									{
										logMessage("[" + api[0] + "] FAILED", LOG_LEVEL.EXCEPTION, true, err);
									}
								}
								#endregion
								break;

							case "GETCOURSELEVELASSIGNMENTDATA":
								#region
								{
									int canvasCourseId = 0;

									for (int i = 1; i <= api.GetUpperBound(0); i++)
									{
										string[] vals = api[i].Split(new char[] { ',' });
										switch (vals[0].ToUpper())
										{
											case "CANVASCOURSEID":
												{
													int.TryParse(vals[1], out canvasCourseId);
												}
												break;

											default:
												{
													logMessage("--> UNUSED VARIABLE: " + vals[0] + ":" + vals[1], LOG_LEVEL.DEBUG, true);
												}
												break;
										}
									}
									try
									{
										dynamic data = await clsAnalyticsApi.getCourseLevelAssignmentData(apiConfig.accessToken, apiConfig.apiUrl, canvasCourseId);
										string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
										logMessage("[" + api[0] + "] Received results: SUCCESS", LOG_LEVEL.DEBUG, true);
									}
									catch (Exception err)
									{
										logMessage("[" + api[0] + "] FAILED", LOG_LEVEL.EXCEPTION, true, err);
									}
								}
								#endregion
								break;

							case "GETCOURSELEVELSTUDENTSUMMARYDATA":
								#region
								{
									int canvasCourseId = 0;

									for (int i = 1; i <= api.GetUpperBound(0); i++)
									{
										string[] vals = api[i].Split(new char[] { ',' });
										switch (vals[0].ToUpper())
										{
											case "CANVASCOURSEID":
												{
													int.TryParse(vals[1], out canvasCourseId);
												}
												break;

											default:
												{
													logMessage("--> UNUSED VARIABLE: " + vals[0] + ":" + vals[1], LOG_LEVEL.DEBUG, true);
												}
												break;
										}
									}
									try
									{
										dynamic data = await clsAnalyticsApi.getCourseLevelStudentSummaryData(apiConfig.accessToken, apiConfig.apiUrl, canvasCourseId);
										string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
										logMessage("[" + api[0] + "] Received results: SUCCESS", LOG_LEVEL.DEBUG, true);
									}
									catch (Exception err)
									{
										logMessage("[" + api[0] + "] FAILED", LOG_LEVEL.EXCEPTION, true, err);
									}
								}
								#endregion
								break;

							case "GETUSERINACOURSEPARTICIPATIONDATA":
								#region
								{
									int canvasCourseId = 0;
									int canvasUserId = 0;

									for (int i = 1; i <= api.GetUpperBound(0); i++)
									{
										string[] vals = api[i].Split(new char[] { ',' });
										switch (vals[0].ToUpper())
										{
											case "CANVASCOURSEID":
												{
													int.TryParse(vals[1], out canvasCourseId);
												}
												break;

											case "CANVASUSERID":
												{
													int.TryParse(vals[1], out canvasUserId);
												}
												break;

											default:
												{
													logMessage("--> UNUSED VARIABLE: " + vals[0] + ":" + vals[1], LOG_LEVEL.DEBUG, true);
												}
												break;
										}
									}
									try
									{
										dynamic data = await clsAnalyticsApi.getUserInACourseParticipationData(apiConfig.accessToken, apiConfig.apiUrl, canvasCourseId, canvasUserId);
										string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
										logMessage("[" + api[0] + "] Received results: SUCCESS", LOG_LEVEL.DEBUG, true);
									}
									catch (Exception err)
									{
										logMessage("[" + api[0] + "] FAILED", LOG_LEVEL.EXCEPTION, true, err);
									}
								}
								#endregion
								break;

							#endregion

							#region COURSES API CALLS
							case "GETCOURSEDETAILS":
								#region
								{
									int canvasCourseId = 0;

									for (int i = 1; i <= api.GetUpperBound(0); i++)
									{
										string[] vals = api[i].Split(new char[] { ',' });
										switch (vals[0].ToUpper())
										{
											case "CANVASCOURSEID":
												{
													int.TryParse(vals[1], out canvasCourseId);
												}
												break;

											default:
												{
													logMessage("--> UNUSED VARIABLE: " + vals[0] + ":" + vals[1], LOG_LEVEL.DEBUG, true);
												}
												break;
										}
									}
									try
									{
										dynamic json = await clsCoursesApi.getCourseDetails(apiConfig.accessToken, apiConfig.apiUrl, canvasCourseId);
										logMessage("[" + api[0] + "] Received results: SUCCESS", LOG_LEVEL.DEBUG, true);
									}
									catch (Exception err)
									{
										logMessage("[" + api[0] + "] FAILED", LOG_LEVEL.EXCEPTION, true, err);
									}
								}
								#endregion
								break;

							case "POSTCREATECOURSE":
								#region
								{
									int accountId = 0;

									for (int i = 1; i <= api.GetUpperBound(0); i++)
									{
										string[] vals = api[i].Split(new char[] { ',' });
										switch (vals[0].ToUpper())
										{
											case "ACCOUNTID":
												{
													int.TryParse(vals[1], out accountId);
												}
												break;

											default:
												{
													logMessage("Add Parameter: " + vals[0] + ":" + vals[1], LOG_LEVEL.DEBUG, true);
													parameters.Add(vals[0], vals[1]);
												}
												break;
										}
									}

									try
									{
										Console.WriteLine("The next API call will create a new course.");
										Console.WriteLine("To continue press <Enter>, to skip this call press any other key: ");
										if (Console.ReadKey().Key == ConsoleKey.Enter)
										{
											dynamic json = await clsCoursesApi.postCreateCourse(apiConfig.accessToken, apiConfig.apiUrl, accountId, parameters);
											logMessage("[" + api[0] + "] Received results: SUCCESS", LOG_LEVEL.DEBUG, true);
										}
									}
									catch (Exception err)
									{
										logMessage("[" + api[0] + "] FAILED", LOG_LEVEL.EXCEPTION, true, err);
									}
								}
								#endregion
								break;

							case "PUTUPDATECOURSE":
								#region
								{
									int canvasCourseId = 0;
									for (int i = 1; i <= api.GetUpperBound(0); i++)
									{
										string[] vals = api[i].Split(new char[] { ',' });
										switch (vals[0].ToUpper())
										{
											case "CANVASCOURSEID":
												{
													int.TryParse(vals[1], out canvasCourseId);
												}
												break;

											default:
												{
													logMessage("Add Parameter: " + vals[0] + ":" + vals[1], LOG_LEVEL.DEBUG, true);
													parameters.Add(vals[0], vals[1]);
												}
												break;
										}
									}

									try
									{
										dynamic json = await clsCoursesApi.putUpdateCourse(apiConfig.accessToken, apiConfig.apiUrl, canvasCourseId, parameters);
										logMessage("[" + api[0] + "] Received results: SUCCESS", LOG_LEVEL.DEBUG, true);
									}
									catch (Exception err)
									{
										logMessage("[" + api[0] + "] FAILED", LOG_LEVEL.EXCEPTION, true, err);
									}

								}
								#endregion
								break;
							#endregion

							#region ENROLLMENTS API CALLS
							case "GETCOURSEENROLLMENTS":
								#region
								{
									try
									{
										int canvasCourseId = 0;
										int canvasSectionId = 0;

										for (int i = 1; i <= api.GetUpperBound(0); i++)
										{
											string[] vals = api[i].Split(new char[] { ',' });
											switch (vals[0].ToUpper())
											{
												case "CANVASCOURSEID":
													{
														int.TryParse(vals[1], out canvasCourseId);
													}
													break;

												case "CANVASSECTIONID":
													{
														int.TryParse(vals[1], out canvasSectionId);
													}
													break;

												default:
													{
														logMessage("--> UNUSED VARIABLE: " + vals[0] + ":" + vals[1], LOG_LEVEL.DEBUG, true);
													}
													break;
											}
										}
										try
										{
											List<dynamic> result = await clsEnrollmentsApi.getCourseEnrollments(apiConfig.accessToken, apiConfig.apiUrl, canvasCourseId, canvasSectionId);
											string json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
											logMessage("[" + api[0] + "] Received results: SUCCESS", LOG_LEVEL.DEBUG, true);
										}
										catch (Exception err)
										{
											logMessage("[" + api[0] + "] FAILED", LOG_LEVEL.EXCEPTION, true, err);
										}
									}
									catch (Exception err)
									{
										logMessage("[" + api[0] + "] FAILED", LOG_LEVEL.EXCEPTION, true, err);
									}
								}
								#endregion
								break;

							case "POSTENROLLUSERINCOURSE":
								#region
								{
									try
									{
										string userId = string.Empty;
										string enrollmentType = string.Empty;
										string enrollmentState = string.Empty;
										int canvasCourseId = 0;
										int canvasSectionId = 0;
										bool sendNotification = false;

										for (int i = 1; i <= api.GetUpperBound(0); i++)
										{
											string[] vals = api[i].Split(new char[] { ',' });
											switch (vals[0].ToUpper())
											{
												case "CANVASCOURSEID":
													{
														int.TryParse(vals[1], out canvasCourseId);
													}
													break;

												default:
													{
														logMessage("Add Parameter: " + vals[0] + ":" + vals[1], LOG_LEVEL.DEBUG, true);
														parameters.Add(vals[0], vals[1]);
													}
													break;
											}
										}

										try
										{
											dynamic json = await clsEnrollmentsApi.postEnrollUserInCourse(apiConfig.accessToken, apiConfig.apiUrl, canvasCourseId, parameters);
											logMessage("[" + api[0] + "]Received results: SUCCESS", LOG_LEVEL.DEBUG, true);
										}
										catch (Exception err)
										{
											logMessage("[" + api[0] + "] FAILED", LOG_LEVEL.EXCEPTION, true, err);
										}
									}
									catch (Exception err)
									{
										logMessage("[" + api[0] + "] FAILED", LOG_LEVEL.EXCEPTION, true, err);
									}
								}
								#endregion
								break;
							#endregion

							#region LOGINS API CALLS
							case "POSTCREATEUSERLOGIN":
								{
									int accountId = 0;

									for (int i = 1; i <= api.GetUpperBound(0); i++)
									{
										string[] vals = api[i].Split(new char[] { ',' });
										switch (vals[0].ToUpper())
										{
											case "ACCOUNTID":
												{
													int.TryParse(vals[1], out accountId);
												}
												break;

											default:
												{
													logMessage("Add Parameter: " + vals[0] + ":" + vals[1], LOG_LEVEL.DEBUG, true);
													parameters.Add(vals[0], vals[1]);
												}
												break;
										}
									}

									try
									{
										Console.WriteLine("Press <Enter> to continue: ");
										if (Console.ReadKey().Key == ConsoleKey.Enter)
										{
											dynamic json = await clsLoginsApi.postCreateUserLogin(apiConfig.accessToken, apiConfig.apiUrl, accountId, parameters);
											logMessage("[" + api[0] + "] Received results: SUCCESS", LOG_LEVEL.DEBUG, true);
										}
									}
									catch (Exception err)
									{
										logMessage("[" + api[0] + "] FAILED", LOG_LEVEL.EXCEPTION, true, err);
									}
								}
								break;

							#endregion

							#region FILE UPLOAD
							case "UPLOADTOFILEFOLDER":
								{
									int folderId = int.MinValue;
									FileInfo fi = null;
									for (int i = 1; i <= api.GetUpperBound(0); i++)
									{
										string[] vals = api[i].Split(new char[] { ',' });
										switch (vals[0].ToUpper())
										{
											case "FOLDERID":
												{
													folderId = int.Parse(vals[1].ToString());
												}
												break;

											case "FILEINFO":
												{
													fi = new FileInfo(vals[1].Replace('/', '\\'));
												}
												break;
										}
									}
									try
									{
										string finalLocation = await clsFileUpload.uploadFileToFolder(apiConfig.accessToken, apiConfig.apiUrl, folderId, fi);
										logMessage("[" + api[0] + "] Received results: SUCCESS - FinalLocation[" + finalLocation + "]", LOG_LEVEL.DEBUG, true);
									}
									catch (Exception err)
									{
										logMessage("[" + api[0] + "] FAILED", LOG_LEVEL.EXCEPTION, true, err);
									}
								}
								break;

							case "UPLOADSUBMISSIONCOMMENTFILE":
								{
									int courseId = 2550;
									int assignmentId = 24147; 
									int userId = 110;
									FileInfo fi = null;

									for (int i = 1; i <= api.GetUpperBound(0); i++)
									{
										string[] vals = api[i].Split(new char[] { ',' });
										switch (vals[0].ToUpper())
										{
											case "COURSEID":
												{
													courseId = int.Parse(vals[1].ToString());
												}
												break;

											case "ASSIGNMENTID":
												{
													assignmentId = int.Parse(vals[1].ToString());
												}
												break;

											case "USERID":
												{
													userId = int.Parse(vals[1].ToString());
												}
												break;

											case "FILEINFO":
											case "FI":
												{
													fi = new FileInfo(vals[1].Replace('/', '\\'));
												}
												break;
										}
									}

									try
									{
										string finalLocation = await clsFileUpload.uploadSubmissionCommentFile(apiConfig.accessToken, apiConfig.apiUrl, courseId, assignmentId, userId, fi);
										logMessage("[" + api[0] + "] Received results: SUCCESS - FinalLocation[" + finalLocation + "]", LOG_LEVEL.DEBUG, true);
									}
									catch (Exception err)
									{
										logMessage("[" + api[0] + "] FAILED", LOG_LEVEL.EXCEPTION, true, err);
									}
								}
								break;

							#endregion

							default:
								{
									logMessage("--> UNKNOWN API CALL [" + api[0] + "] - VERIFY CONFIG FILE FORMAT", LOG_LEVEL.DEBUG, true);
								}
								break;
						}
					}
				}
			}
			else
			{
				logMessage("skip API execution", LOG_LEVEL.DEBUG, true);
			}
			working = false;
			return "complete";
		}
	}
}
