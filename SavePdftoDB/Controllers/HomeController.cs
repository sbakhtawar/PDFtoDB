using SavePdftoDB.Models;  
using System;  
using System.Collections.Generic;  
using System.IO;  
using System.Linq;  
using System.Web;  
using System.Web.Mvc;  
using Dapper;  
using System.Configuration;  
using System.Data.SqlClient;  
using System.Data; 

namespace SavePdftoDB.Controllers
{
    public class HomeController : Controller
    {
        private SqlConnection con;
        private string constr;
        private void DbConnection()
        {
            constr = ConfigurationManager.ConnectionStrings["dbcon"].ToString();
            con = new SqlConnection(constr);
        }
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase[] files)
        {
            foreach (HttpPostedFileBase file in files)
            {
                String FileExt = Path.GetExtension(file.FileName).ToUpper();

                if (FileExt == ".PDF")
                {
                    Stream str = file.InputStream;
                    BinaryReader Br = new BinaryReader(str);
                    Byte[] FileDet = Br.ReadBytes((Int32)str.Length);

                    FileDetailsModel Fd = new FileDetailsModel();
                    Fd.FileName = file.FileName;
                    Fd.FileContent = FileDet;
                    Fd.ContentType = file.ContentType;
                    Fd.ContentLength = file.ContentLength;
                    SaveFileDetails(Fd);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("About");
        }
        
        private void SaveFileDetails(FileDetailsModel objDet)
        {
            DynamicParameters Parm = new DynamicParameters();
            Parm.Add("@FileName", objDet.FileName);
            Parm.Add("@FileContent", objDet.FileContent);
            Parm.Add("@ContentType", objDet.ContentType);
            Parm.Add("@ContentLength", objDet.ContentLength.ToString());
            DbConnection();
            con.Open();
            con.Execute("AddFileDetails", Parm, commandType: CommandType.StoredProcedure);
            con.Close();
        }

        [HttpGet]
        public PartialViewResult About()
        {
            List<FileDetailsModel> DetList = GetFileList();
            return PartialView("About", DetList);
        }

        private List<FileDetailsModel> GetFileList()
        {
            List<FileDetailsModel> DetList = new List<FileDetailsModel>();
            DbConnection();
            con.Open();
            DetList = SqlMapper.Query<FileDetailsModel>(con, "GetFileDetails", commandType: CommandType.StoredProcedure).ToList();
            con.Close();
            return DetList;
        }

        [HttpGet]
        public FileResult DownLoadFile(int id)
        {
            List<FileDetailsModel> ObjFiles = GetFileList();
            var FileById = (from FC in ObjFiles where FC.Id.Equals(id) select new { FC.FileName, FC.FileContent }).ToList().FirstOrDefault();
            return File(FileById.FileContent, "application/pdf", FileById.FileName);
        }

        [HttpGet]
        public ActionResult DeleteFile(int id)
        {
            IDataReader reader = null;
            DbConnection();
            con.Open();
            SqlCommand cmd = new SqlCommand("DeleteFileDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            reader = cmd.ExecuteReader();
            return RedirectToAction("About");
        }
    }
}