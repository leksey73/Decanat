using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DECANAT.ModelData;
using DECANAT.Repozitory;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace DECANAT.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private string ParseOracleError(string error)
        {
            return error.Split(':').ElementAt(1).Split('O').ElementAt(0);
        }
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #region FACULTY
        public ActionResult Facultes()
        {
            return View(UnitOfWork.Faculties.GetAll());
        }
        public ActionResult AddFaculty()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddFaculty(NewFaculty faculty)
        {
            try
            {
                UnitOfWork.Faculties.Create(faculty);
                return RedirectToAction("Facultes");
            }
            catch
            {
                ModelState.AddModelError("Name", "?????? ??????????, ???????? ????? ????????? ??? ?????");
                return View(faculty);
            }
        }
        public ActionResult EditFaculty(int id)
        {
            Faculty oldFaculty = UnitOfWork.Faculties.Get(id);
            NewFaculty faculty = new NewFaculty { Name = oldFaculty.Name };
            return View(faculty);
        }
        [HttpPost]
        public ActionResult EditFaculty(NewFaculty faculty)
        {
            try
            {
                UnitOfWork.Faculties.Update(faculty);
            }
            catch
            {
                ModelState.AddModelError("NewName", "?? ??????? ????????????? ?????????, ????????, ????????? ? ????? ?????? ??? ?????");
                return View(faculty);
            }
            return RedirectToAction("Facultes");
        }
        public ActionResult DeleteFaculty(int id)
        {
            Faculty oldFaculty = UnitOfWork.Faculties.Get(id);
            Faculty faculty = new Faculty { Name = oldFaculty.Name };
            return View(faculty);
        }
        [HttpPost]
        public ActionResult DeleteFaculty(Faculty faculty)
        {
            try
            {
                UnitOfWork.Faculties.Delete(faculty.id);
            }
            catch(Exception)
            {
                ModelState.AddModelError("Name", "?????????? ??????? ???? ?????????, ??? ??? ?? ?? ??????");
            }
            if (ModelState.IsValid)
            {
                return RedirectToAction("Facultes");
            }
            else
            {
                return View();
            }
        }
        #endregion
        #region SPECIALITIS
        public ActionResult Specialitis(int faculty_id)
        {
            ViewBag.faculty_id = faculty_id;
            ViewBag.faculty = UnitOfWork.Faculties.Get(faculty_id).Name;
            return View(UnitOfWork.Specialitys.GetAll("where \"???_??????????\"=" + faculty_id));
        }
        public ActionResult AddSpeciality(int faculty_id)
        {
            Faculty faculty = UnitOfWork.Faculties.Get(faculty_id);
            Speciality speciality = new Speciality { faculty_code = faculty_id, faculty_name = faculty.Name };
            return View(speciality);
        }
        [HttpPost]
        public ActionResult AddSpeciality(Speciality speciality)
        {
            try
            {
                UnitOfWork.Specialitys.Create(speciality);
                return RedirectToAction("Specialitis", new { faculty_id = speciality.faculty_code });
            }
            catch
            {
                ModelState.AddModelError("speciality_name", "?????? ??????????, ???????? ????? ????????????? ??? ?????");
                return View(speciality);
            }

        }
        public ActionResult EditSpeciality(int id)
        {
            NewSpeciality speciality = UnitOfWork.Specialitys.Get(id);
            speciality.new_speciality_number = speciality.speciality_number;
            speciality.new_speciality_name = speciality.speciality_name;
            return View(speciality);
        }
        [HttpPost]
        public ActionResult EditSpeciality(NewSpeciality speciality)
        {
            try
            {
                UnitOfWork.Specialitys.Update(speciality);
                return RedirectToAction("Specialitis", new { faculty_id = speciality.faculty_code });
            }
            catch { return View(speciality); }
        }
        public ActionResult DeleteSpeciality(int id)
        {
            Speciality speciality = UnitOfWork.Specialitys.Get(id);
            return View(speciality);
        }
        [HttpPost]
        public ActionResult DeleteSpeciality(Speciality speciality)
        {
            try
            {
                UnitOfWork.Specialitys.Delete(speciality.id);
                return RedirectToAction("Specialitis", new { faculty_id = speciality.faculty_code });
            }
            catch (Exception)
            {
                ModelState.AddModelError("speciality_name", "?????????? ??????? ??? ?????????????, ??? ??? ??? ?? ??????");
                return View(speciality);
            }
        }
        #endregion
        #region DISCIPLINES
        public ActionResult Disciplines(int speciality_id)
        {
            Speciality speciality = UnitOfWork.Specialitys.Get(speciality_id);
            ViewBag.faculty_id = speciality.faculty_code;
            ViewBag.speciality_id = speciality_id;
            ViewBag.faculty = speciality.faculty_name;
            ViewBag.speciality_name = speciality.speciality_name;
            ViewBag.speciality_number = speciality.speciality_number;
            return View(UnitOfWork.Disciplines.GetAll("where \"???_?????????????\"=" + speciality_id));
        }
        public ActionResult AddDiscipline(int speciality_id)
        {
            Speciality speciality = UnitOfWork.Specialitys.Get(speciality_id);
            Discipline discipline = new Discipline
            {
                faculty_name = speciality.faculty_name,
                faculty_code = speciality.faculty_code,
                speciality_code = speciality.id,
                speciality_name = speciality.speciality_name,
                speciality_number = speciality.speciality_number
            };
            return View(discipline);
        }
        [HttpPost]
        public ActionResult AddDiscipline(Discipline discipline)
        {
            try
            {
                UnitOfWork.Disciplines.Create(discipline);
                return RedirectToAction("Disciplines", new { speciality_id = discipline.speciality_code });
            }
            catch
            {
                ModelState.AddModelError("discipline_name", "?????? ??????????, ???????? ????? ?????????? ??? ?????");
                return View(discipline);
            }

        }
        public ActionResult EditDiscipline(int id)
        {
            NewDiscipline discipline = UnitOfWork.Disciplines.Get(id);
            return View(discipline);
        }
        [HttpPost]
        public ActionResult EditDiscipline(NewDiscipline discipline)
        {
            try
            {
                UnitOfWork.Disciplines.Update(discipline);
                return RedirectToAction("Disciplines", new { speciality_id = discipline.speciality_code });
            }
            catch
            {
                ModelState.AddModelError("newDisciplineName", "?????? ??????????????, ???????? ????? ?????????? ??? ?????");
                return View(discipline);
            }
        }
        public ActionResult DeleteDiscipline(int id)
        {
            Discipline discipline = UnitOfWork.Disciplines.Get(id);
            return View(discipline);
        }
        [HttpPost]
        public ActionResult DeleteDiscipline(Discipline discipline)
        {
            try
            {
                UnitOfWork.Disciplines.Delete(discipline.id);
                return RedirectToAction("Disciplines", new { speciality_id = discipline.speciality_code });
            }
            catch (Exception)
            {
                ModelState.AddModelError("discipline_name", "?????????? ??????? ??? ??????????, ??? ??? ??? ?? ??????");
                return View(discipline);
            }
        }
        #endregion
        #region GROUPS
        public ActionResult Groups(int speciality_id)
        {
            Speciality speciality = UnitOfWork.Specialitys.Get(speciality_id);
            ViewBag.faculty_id = speciality.faculty_code;
            ViewBag.faculty = speciality.faculty_name;
            ViewBag.speciality_name = speciality.speciality_name;
            ViewBag.speciality_number = speciality.speciality_number;
            ViewBag.speciality_id = speciality.id;
            return View(UnitOfWork.Groups.GetAll("where \"???_?????????????\"=" + speciality_id));
        }
        public ActionResult AddGroup(int speciality_id)
        {
            Speciality speciality = UnitOfWork.Specialitys.Get(speciality_id);
            Group group = new Group
            {
                faculty_name = speciality.faculty_name,
                speciality_number = speciality.speciality_number,
                speciality_name = speciality.speciality_name,
                speciality_id = speciality.id
            };
            return View(group);
        }
        [HttpPost]
        public ActionResult AddGroup(Group group)
        {
            try
            {
                UnitOfWork.Groups.Create(group);
                return RedirectToAction("Groups", new { speciality_id = group.speciality_id });
            }
            catch
            {
                ModelState.AddModelError("group_number", "?????? ??????????, ???????? ????? ?????? ??? ?????");
                return View(group);
            }

        }
        public ActionResult DeleteGroup(int id)
        {
            return View(UnitOfWork.Groups.Get(id));
        }
        [HttpPost]
        public ActionResult DeleteGroup(Group group)
        {
            try
            {
                UnitOfWork.Groups.Delete(group.id);
                return RedirectToAction("Groups", new { speciality_id = group.speciality_id });
            }
            catch (Exception)
            {
                ModelState.AddModelError("group_number", "?????? ????????, ?????? ?????? ?? ?????");
                return View(group);
            }
        }
        #endregion
       
        #region STUDENTS
        public ActionResult Students(int group_id)
        {
            Group group = UnitOfWork.Groups.Get(group_id);
            ViewBag.speciality_id = group.speciality_id;
            ViewBag.group_id = group_id;
            ViewBag.faculty = group.faculty_name;
            ViewBag.speciality_name = group.speciality_name;
            ViewBag.speciality_number = group.speciality_number;
            ViewBag.coors = group.coors;
            ViewBag.group_number = group.group_number;
           
            IEnumerable<Group> groups = UnitOfWork.Students.GetAll("where \"???_??????\"=" + group_id);
            return View(groups);
        }
        public ActionResult AddStudent(int group_id)
        {
            Group group = UnitOfWork.Groups.Get(group_id);
            Student student = new Student
            {
                speciality_number = group.speciality_number,
                faculty_name = group.faculty_name,
                speciality_name = group.speciality_name,
                group_number = group.group_number,
                coors = group.coors,
                group_id = group_id
            };
            return View(student);
        }
        [HttpPost]
        public ActionResult AddStudent(Student student)
        {
            try
            {
                UnitOfWork.Students.Create(student);
                return RedirectToAction("Students", new { group_id = student.group_id });
            }
            catch
            {
                ModelState.AddModelError("FIO", "?????? ??????????, ???????? ????? ??????? ??? ?????");
                return View(student);
            }

        }
        public ActionResult EditStudent(int id)
        {
            NewStudent student = UnitOfWork.Students.Get(id);
            return View(student);
        }
        [HttpPost]
        public ActionResult EditStudent(NewStudent student)
        {
            try
            {
                UnitOfWork.Students.Update(student);
                return RedirectToAction("Students", new { group_id = student.group_id });
            }
            catch
            {
                ModelState.AddModelError("FIO", "?????? ??????????????, ???????? ????? ??????? ??? ?????");
                return View(student);
            }
        }
        public ActionResult DeleteStudent(int id)
        {
            Student student = UnitOfWork.Students.Get(id);
            return View(student);
        }
        [HttpPost]
        public ActionResult DeleteStudent(Student student)
        {
            try
            {
                UnitOfWork.Students.Delete(student.id);
                return RedirectToAction("Students", new { group_id = student.group_id });
            }
            catch (Exception)
            {
                ModelState.AddModelError("FIO", "?????? ???????? ????????");
                return View(student);
            }
        }
        #endregion
        #region WORKS
        public ActionResult Works(int group_id)
        {
            Group group = UnitOfWork.Groups.Get(group_id);
            ViewBag.speciality_id = group.speciality_id;
            ViewBag.group_id = group.id;
            ViewBag.speciality_name = group.speciality_name;
            ViewBag.speciality_number = group.speciality_number;
            ViewBag.coors = group.coors;
            ViewBag.group_number = group.group_number;
            ViewBag.group_id = group.id;
            return View(UnitOfWork.Works.GetAll("where \"???_??????\"=" + group_id));
        }
        public ActionResult AddWork(int group_id)
        {
            Group group = UnitOfWork.Groups.Get(group_id);
            NewWork workload = new NewWork();
            workload.work = new Work
            {
                faculty_name = group.faculty_name,
                speciality_name = group.speciality_name,
                speciality_number = group.speciality_number,
                group_id = group.id,
                group_number = group.group_number,
                coors = group.coors
            };
            workload.teachers = UnitOfWork.Works.GetTeachers();
            workload.disciplines = UnitOfWork.Works.GetDisciplinesFromSpecialityId(group.speciality_id);
            return View(workload);
        }
        [HttpPost]
        public ActionResult AddWork(Work work)
        {
            try
            {
                UnitOfWork.Works.Create(work);
                return RedirectToAction("Works", new { group_id = work.group_id });
            }
            catch (Exception e)
            {
                ModelState.AddModelError("error", ParseOracleError(e.Message));
                NewWork workload = new NewWork();
                workload.work = work;
                workload.teachers = UnitOfWork.Works.GetTeachers();
                workload.disciplines = UnitOfWork.Works.GetDisciplinesFromSpecialityId(work.speciality_id);
                return View(workload);
            }

        }
        public ActionResult DeleteWork(int id)
        {
            Work work = UnitOfWork.Works.Get(id);
            return View(work);
        }
        [HttpPost]
        public ActionResult DeleteWork(Work work)
        {
            try
            {
                UnitOfWork.Works.Delete(work.id);
                return RedirectToAction("Works", new { group_id = work.group_id });
            }
            catch (Exception e)
            {
                ModelState.AddModelError("error", ParseOracleError(e.Message));
                return View(work);
            }
        }
        #endregion
        #region TEACHERS
        public ActionResult Teachers()
        {
            return View(UnitOfWork.Teachers.GetAllTeachers(UserManager));
        }
        public ActionResult EditTeacher(string id)
        {
            NewTeacher student = UnitOfWork.Teachers.Get(UserManager, id);
            return View(student);
        }
        [HttpPost]
        public ActionResult EditTeacher(NewTeacher teacher)
        {
            try
            {
                UnitOfWork.Teachers.Update(UserManager, teacher);
                return RedirectToAction("Teachers");
            }
            catch
            {
                ModelState.AddModelError("new_username", "?????? ??????????????");
                return View(teacher);
            }
        }
        public ActionResult DeleteTeacher(string id)
        {
            Teacher teacher = UnitOfWork.Teachers.Get(UserManager, id);
            return View(teacher);
        }
        [HttpPost]
        public ActionResult DeleteTeacher(Teacher teacher)
        {
            try
            {
                UnitOfWork.Teachers.Delete(UserManager, teacher);
                return RedirectToAction("Teachers");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("error", ParseOracleError(e.Message));
                return View(teacher);
            }
        }
        #endregion
        #region ROLES
        public ActionResult Users()
        {
            return View(UnitOfWork.Teachers.GetAll(UserManager));
        }
        public ActionResult Roles(string id)
        {
            UserRole roles = UnitOfWork.Roles.Get(UserManager, id);
            return View(roles);
        }
        public ActionResult DeleteRole(string id, string role)
        {
            UnitOfWork.Roles.Delete(UserManager, id, role);
            return RedirectToAction("Roles", new { id = id });
        }
        public ActionResult AddRole(string id)
        {
            NewUserRole user = UnitOfWork.Roles.Get(UserManager, id);
            user.roles_list = UnitOfWork.Roles.GetAllRoles();
            return View(user);
        }
        [HttpPost]
        public ActionResult AddRole(NewUserRole user)
        {
            try
            {
                UnitOfWork.Roles.Add(UserManager, user.id, user.new_role);
                return RedirectToAction("Roles", new { id = user.id });
            }
            catch
            {
                ModelState.AddModelError("new_role", "?????? ?????????? ????");
                return View(user);
            }
        }
        #endregion
    }
}