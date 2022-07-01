using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static DynamicQuestionnaireForm.Models.QuestModels;

namespace DynamicQuestionnaireForm.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // 資料庫連線字串
            string cnStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

            // 輸出物件
            List<QuestForm> outModel = new List<QuestForm>();

            using (var cn = new SqlConnection(cnStr))
            {
                // 取得問卷題目
                var questTitle = cn.Query<QuestForm>("SELECT * FROM QuestForm ORDER BY QuestNo");

                // 取得問卷選項
                var questOtion = cn.Query<QuestOption>("SELECT * FROM QuestOption ORDER BY QuestNo,QuestOtpionId");

                foreach (var item in questTitle)
                {
                    // 建立問卷題目+選項
                    QuestForm form = new QuestForm();
                    form.QuestNo = item.QuestNo;
                    form.QuestTitle = item.QuestTitle;
                    form.QuestOptionType = item.QuestOptionType;
                    // 加入問卷選項
                    form.QuestOptions = questOtion.Where(x => x.QuestNo == item.QuestNo).ToList();
                    outModel.Add(form);
                }
            }

            // 轉為 Json 傳至前端
            ViewData["QuestForm"] = JsonConvert.SerializeObject(outModel);

            return View();
        }

        /// <summary>
        /// 儲存使用者答案
        /// </summary>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        public ActionResult SaveUserAnswer(UserAnswerModel inModel)
        {
            Hashtable outModel = new Hashtable();

            // 檢查輸入來源
            if (inModel.QuestUserAnswer.Count == 0)
            {
                outModel["ErrMsg"] = "無使用者答案";
                return Json(outModel);
            }

            // 取得資料庫連線字串
            string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

            // 當程式碼離開 using 區塊時，會自動關閉連接
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                // 資料庫連線
                conn.Open();

                //測試使用者ID
                string testUserId = "Mars";

                // 填寫問卷時間
                string AnswerTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                foreach (UserAnswerObj userAnswer in inModel.QuestUserAnswer)
                {
                    // 寫入使用者答案
                    string sql = @"INSERT INTO QuestUserAnswer(QuestNo ,UserId ,AnswerTime ,UserAnswer) VALUES (@QuestNo, @UserId ,@AnswerTime ,@UserAnswer)";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sql;

                    // 使用參數化填值
                    cmd.Parameters.AddWithValue("@QuestNo", userAnswer.QuestNo);
                    cmd.Parameters.AddWithValue("@UserId", testUserId);
                    cmd.Parameters.AddWithValue("@AnswerTime", AnswerTime);
                    cmd.Parameters.AddWithValue("@UserAnswer", userAnswer.UserAnswer);

                    // 執行資料庫更新動作
                    cmd.ExecuteNonQuery();
                }
            }

            outModel["ResultMsg"] = "儲存完成";
            // 回傳 Json 給前端
            return Json(outModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}