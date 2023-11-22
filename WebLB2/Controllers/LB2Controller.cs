using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LB2.Models;
using LB2.Models.ViewModels;
using System.Security.Cryptography;
using System.Web.Security;
using System.Security.Principal;
using System.Text;

namespace WebLB2.Controllers
{
    public class LB2Controller : Controller
    {
        [AllowAnonymous]
        public ActionResult ListOfStudents()
        {
            List<Студенты> students = new List<Студенты>();
            using( var db= new VMamontovEntities())
            {
             
                students = db.Студенты.OrderByDescending(x => x.Имя)
                    .ThenBy(x => x.Фамилия).ToList();
               
            }
            return View(students);
        }
        [Authorize]
        public ActionResult StudentDetails(Guid personId)
        {
            Студенты model = new Студенты();
            using (var db = new VMamontovEntities())
            {
                model = db.Студенты.Find(personId);
            }
                return View(model);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpGet]
        [Authorize(Roles ="Admin")]
        public ActionResult CreateStudent()
        {
            ViewBag.Genders = new SelectList(GetGenderList(), "Item1", "Item2");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult CreateStudent(Студенты newStudent)
        {
            if (ModelState.IsValid)
            {
                using (var context = new VMamontovEntities())
                {
                    Студенты студент = new Студенты
                    {
                        ID_студента = Guid.NewGuid(),
                        Фамилия = newStudent.Фамилия,
                        Имя= newStudent.Имя,
                        Отчество = newStudent.Отчество,
                        Пол= newStudent.Пол,
                        Адрес_проживания= newStudent.Адрес_проживания,
                        Дата_рождения= newStudent.Дата_рождения,
                        Уровень_владения_ИЯ= newStudent.Уровень_владения_ИЯ,
                        InsertedDateTime = newStudent.InsertedDateTime,
                        WakeUpTime = newStudent.WakeUpTime,
                        GraduationDate = newStudent.GraduationDate
                    };
                    context.Студенты.Add(студент);
                    context.SaveChanges();
             
                    
                }
                return RedirectToAction("ListOfStudents");
            }
            ViewBag.Genders = new SelectList(GetGenderList(), "Item1", "Item2");
            return View(newStudent);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditStudent(Guid studentID)
        {
            Студенты model;
            using (var context = new VMamontovEntities())
            {
                Студенты student = context.Студенты.Find(studentID);
                model = new Студенты
                {
                    ID_студента = student.ID_студента,
                    Фамилия = student.Фамилия,
                    Имя = student.Имя,
                    Отчество = student.Отчество,
                    Пол = student.Пол,
                    Адрес_проживания = student.Адрес_проживания,
                    Дата_рождения = student.Дата_рождения,
                    Уровень_владения_ИЯ = student.Уровень_владения_ИЯ,
                    InsertedDateTime = student.InsertedDateTime,
                    WakeUpTime = student.WakeUpTime,
                    GraduationDate = student.GraduationDate
                };

            }
            ViewBag.Genders = new SelectList(GetGenderList(), "Item1", "Item2");
            return View(model);
        }


        [HttpPost]
        public ActionResult EditStudent(Студенты newStudent)
        {
            if (ModelState.IsValid)
            {
                using (var context = new VMamontovEntities())
                {
                    Студенты студент = new Студенты
                    {
                        ID_студента = newStudent.ID_студента,
                        Фамилия = newStudent.Фамилия,
                        Имя = newStudent.Имя,
                        Отчество = newStudent.Отчество,
                        Пол = newStudent.Пол,
                        Адрес_проживания = newStudent.Адрес_проживания,
                        Дата_рождения = newStudent.Дата_рождения,
                        Уровень_владения_ИЯ = newStudent.Уровень_владения_ИЯ,
                            InsertedDateTime = newStudent.InsertedDateTime,
                        WakeUpTime = newStudent.WakeUpTime,
                        GraduationDate = newStudent.GraduationDate
                    };
                    context.Студенты.Attach(студент);
                    context.Entry(студент).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();


                }
                return RedirectToAction("ListOfStudents");
            }
            ViewBag.Genders = new SelectList(GetGenderList(), "Item1", "Item2");
            return View(newStudent);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteStudent(Guid studentID)
        {
            Студенты deleteStudent;
            using (var context = new VMamontovEntities())
            {
                deleteStudent = context.Студенты.Find(studentID);
            }
            return View(deleteStudent);
            }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(UserVM webUser)
        {
            if (ModelState.IsValid)
            {
                using (var context = new VMamontovEntities())
                {
                    User user = null;
                    user = context.User.Where(u => u.Login == webUser.Login).FirstOrDefault();
                    if (user != null)
                    {
                        string PasswordHash = ReturnHashCode(webUser.Password + user.Salt.ToString().ToUpper());
                        if (PasswordHash == user.PasswordHash)
                        {
                            string userRole = "";
                            switch (user.UserRole)
                            {
                                case 2:
                                    userRole = "User";
                                    break;
                                case 3:
                                    userRole = "Admin";
                                    break;
                            }
                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                                1, user.Login, DateTime.Now, DateTime.Now.AddDays(1), false, userRole);
                            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                            HttpContext.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket));
                            return RedirectToAction("ListOfStudents");
                        }
                    }
                }
            }
            ViewBag.Error = "Пользователя с таким логином и паролем не существует, попробуйте ещё";
            return View(webUser);
            }
        string ReturnHashCode(string LoginAndSalt)
        {
            string hash = "";
            using (SHA1 sha1Hash = SHA1.Create())
            {
                byte[] data = sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(LoginAndSalt));
                StringBuilder sBuilder = new StringBuilder();
                for (int i=0; i<data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                hash = sBuilder.ToString().ToUpper();
            }
            return hash;
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("ListOfStudents");
        }
        [HttpPost, ActionName("DeleteStudent")]
        public ActionResult DeleteConfirmed(Guid studentID)
        {
            Студенты deleteStudent;
            using (var context = new VMamontovEntities())
            {
                deleteStudent = context.Студенты.Find(studentID);
                context.Студенты.Remove(deleteStudent);
                context.SaveChanges();
            }
            return RedirectToAction("ListOfStudents");
            }
        [ChildActionOnly]
        public ActionResult GraduationDate(Guid studentID)
        {
            string message = "";
            using (var context = new VMamontovEntities())
            {
                string graduationDate = context.Студенты.Find(studentID).GraduationDate.ToString();
                message = "Дата выпуска: " + graduationDate;
            }
            return PartialView("", message);
            }
        [ValidateAntiForgeryToken]
        List<Tuple<bool,string>> GetGenderList()
        {
            List<Tuple<bool, string>> genders = new List<Tuple<bool, string>>
            {
                new Tuple<bool, string>(false,"Женский"),
                new Tuple<bool, string>(true,"Мужской"),
            };
            return genders;
        }
    }
}