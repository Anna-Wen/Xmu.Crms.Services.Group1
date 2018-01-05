/*************/
/*************/
/*@author 1-4*/
/*************/
/*************/
using System;
using System.Collections.Generic;
using System.Text;
using Xmu.Crms.Services.Group1.Dao;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;
using Xmu.Crms.Shared.Exceptions;

namespace Xmu.Crms.Services.Group1
{
    class UserService : IUserService
    {
        /// <summary>
        /// @author Group 1-4
        /// @version 2.00
        /// </summary>
        private readonly IUserDao _userDao;

        public UserService(IUserDao userDao)
        {
            _userDao = userDao;
        }

        /// <summary>
        /// 根据用户Id获取用户的信息.
        /// @author Group 1-4
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>user 用户信息</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISchoolService.GetSchoolBySchoolId(System.Int64)"/>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.UserNotFoundException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.UserNotFoundException">未找到对应用户</exception>
        public UserInfo GetUserByUserId(long userId)
        {
            if (userId.GetType().ToString() != "System.Int64")
                throw new UserNotFoundException();//id格式错误 
            try
            {
                UserInfo userInfo = _userDao.Find(userId);
                return userInfo;
            }
            catch(UserNotFoundException e)
            {
                throw e;
            }            
        }

        /// <summary>
        /// 根据用户学（工）号获取用户的信息.
        /// @author Group 1-4
        /// </summary>
        /// <param name="userNumber">用户学（工）号</param>
        /// <returns>user 用户信息</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISchoolService.GetSchoolBySchoolId(System.Int64)"/>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.UserNotFoundException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.UserNotFoundException">未找到对应用户</exception>
        public UserInfo GetUserByUserNumber(string number)
        {
            try
            {
                return _userDao.GetByNumber(number);
            }
            catch(UserNotFoundException)
            {
                throw;
            }
        }

        // InsertAttendanceById中用到的方法
        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }
        /// <summary>
        /// 添加学生签到信息.
        /// @author Group 1-4
        /// 根据班级id，讨论课id，学生id，经度，纬度进行签到 在方法中通过班级id，讨论课id获取当堂课发起签到的位置
        /// </summary>
        /// <param name="classId">班级的id</param>
        /// <param name="seminarId">讨论课的id</param>
        /// <param name="userId">学生的id</param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.ClassNotFoundException">未找到班级</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.SeminarNotFoundException">未找到讨论课</exception>
        public void InsertAttendanceById(long classId, long seminarId, long userId, double longitude, double latitude)
        {
            if (seminarId.GetType().ToString() != "System.Int64" || classId.GetType().ToString() != "System.Int64"
                || userId.GetType().ToString() != "System.Int64")
                throw new ArgumentException();//id格式错误
            Location location = _userDao.FindTeacherLocation(classId, seminarId);
           
            double tLongtitude = (double)location.Longitude;
            double tLatitude = (double)location.Latitude;
            double EARTH_RADIUS = 6378.137;//地球半径
            double radLat1 = rad(tLatitude);
            double radLat2 = rad(latitude);
            double a = radLat1 - radLat2;
            double b = rad(tLongtitude) - rad(longitude);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;//s为经纬度换算出的实际距离（单位km）
            Attendance attendance = new Attendance();
            attendance.ClassInfo.Id = classId;
            attendance.Seminar.Id = seminarId;
            if(s<0.1 && location.Status==1)   
                attendance.AttendanceStatus = AttendanceStatus.Present;
            else if(s<0.1 && location.Status==0)
                attendance.AttendanceStatus = AttendanceStatus.Late;
            _userDao.AddAttendance(attendance);

        }

        /// <summary>
        /// 获取讨论课所在班级缺勤学生名单.
        /// </summary>
        /// <param name="seminarId">讨论课ID</param>
        /// <param name="classId">班级ID</param>
        /// <returns>list 处于缺勤状态的学生列表</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IUserService.ListUserByClassId(System.Int64,System.String,System.String)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IUserService.ListPresentStudent(System.Int64,System.Int64)"/>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.ClassNotFoundException">未找到对应班级</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.SeminarNotFoundException">未找到对应讨论课</exception>
        public IList<UserInfo> ListAbsenceStudent(long seminarId, long classId)
        {
            if (seminarId.GetType().ToString() != "System.Int64" || classId.GetType().ToString() != "System.Int64")
                throw new ArgumentException();//id格式错误
            IList<UserInfo> list  = _userDao.FindAbsenceStudents(seminarId, classId);
            if (list == null)
            {
                throw new ClassNotFoundException();//未找到对应班级
                throw new SeminarNotFoundException();//未找到对应讨论课
            }
            return list;
        }

        /// <summary>
        /// 获取学生签到信息.
        /// @author Group 1-4
        /// 根据班级id，讨论课id获取当堂课签到信息
        /// </summary>
        /// <param name="classId">班级的id</param>
        /// <param name="seminarId">讨论课id</param>
        /// <returns>list 当堂课签到信息</returns>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.ClassNotFoundException">未找到班级</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.SeminarNotFoundException">未找到讨论课</exception>
        public IList<Attendance> ListAttendanceById(long classId, long seminarId)
        {
            if (seminarId.GetType().ToString() != "System.Int64" || classId.GetType().ToString() != "System.Int64")
                throw new ArgumentException();//id格式错误
            IList < Attendance > list= _userDao.FindAttendanceById(seminarId, classId);
            if(list==null)
            {
                throw new ClassNotFoundException();//未找到对应班级
                throw new SeminarNotFoundException();//未找到对应讨论课
            }
            return list;
        }

        /// <summary>
        /// 获取讨论课所在的班级的迟到学生名单.
        /// @author Group 1-4
        /// </summary>
        /// <param name="seminarId">讨论课ID</param>
        /// <param name="classId">班级ID</param>
        /// <returns>list 处于迟到状态的学生的列表</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IUserService.ListAttendanceById(System.Int64,System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IUserService.GetUserByUserId(System.Int64)"/>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.ClassNotFoundException">未找到对应班级</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.SeminarNotFoundException">未找到对应讨论课</exception>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        public IList<UserInfo> ListLateStudent(long seminarId, long classId)
        {
            if (seminarId.GetType().ToString() != "System.Int64" || classId.GetType().ToString() != "System.Int64")
                throw new ArgumentException();//id格式错误
            IList<UserInfo> list = _userDao.FindLateStudents(seminarId, classId);
            if (list == null)
            {
                throw new ClassNotFoundException();//未找到对应班级
                throw new SeminarNotFoundException();//未找到对应讨论课
            }
            return list;
        }

        /// <summary>
        /// 获取讨论课所在的班级的出勤学生名单.
        /// @author Group 1-4
        /// </summary>
        /// <param name="seminarId">讨论课ID</param>
        /// <param name="classId">班级ID</param>
        /// <returns>list 处于出勤状态的学生的列表</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IUserService.ListAttendanceById(System.Int64,System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IUserService.GetUserByUserId(System.Int64)"/>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        public IList<UserInfo> ListPresentStudent(long seminarId, long classId)
        {
            if (seminarId.GetType() != typeof(long) || classId.GetType() != typeof(long)) throw new ArgumentException();
            IList<Attendance> AttendanceList = ListAttendanceById(classId, seminarId);
            IList<UserInfo> PresentStudentList = new List<UserInfo>();
            foreach (Attendance Temp in AttendanceList)
            {
                if (Temp.AttendanceStatus == 0)
                    PresentStudentList.Add(GetUserByUserId(Temp.Id));
            }
            if (PresentStudentList == null) throw new SeminarNotFoundException();

            return PresentStudentList;

        }

        /// <summary>
        /// 按班级ID、学号开头、姓名开头获取学生列表.
        /// @author Group 1-4
        /// </summary>
        /// <param name="classId">班级ID</param>
        /// <param name="numBeginWith">学号开头</param>
        /// <param name="nameBeginWith">姓名开头</param>
        /// <returns>list 用户列表</returns>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.ClassNotFoundException">未找到对应班级</exception>
        public IList<UserInfo> ListUserByClassId(long classId, string numBeginWith, string nameBeginWith)
        {
            try
            {
                if (classId.GetType() != typeof(long) || classId < 0/* || numBeginWith.GetType() != typeof(string) || nameBeginWith.GetType() != typeof(string)*/) throw new ArgumentException();
                IList<UserInfo> StudentInfoList = _userDao.ListUserByClassId(classId, numBeginWith, nameBeginWith);
                if (StudentInfoList == null) throw new ClassNotFoundException();
                return StudentInfoList;
            }catch
            {
                throw;
            }
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 根据用户名获取用户列表.
        /// @author Group 1-4
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>list 用户列表</returns>
        public IList<UserInfo> ListUserByUserName(string userName)
        {
            if (userName.GetType() != typeof(string)) throw new ArgumentException();
            IList<UserInfo> userList = _userDao.ListUserByUserName(userName);
            if (userList == null) throw new UserNotFoundException();
            return userList;

        }

        /// <summary>
        /// 根据用户名获取用户ID.
        /// @author Group 1-4
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>userId 用户ID</returns>
        public IList<long> ListUserIdByUserName(string userName)
        {
            if (userName.GetType() != typeof(string)) throw new ArgumentException();
            IList<long> userIdList = _userDao.ListUserIdByUserName(userName);
            if (userIdList == null) throw new UserNotFoundException();
            return userIdList;

        }

        /// <summary>
        /// 根据用户ID修改用户信息.
        /// @author Group 1-4
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="user">用户信息</param>
        /// <returns>list 用户id列表</returns>
        /// <exception cref="T:System.ArgumentException">id格式错误</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.UserNotFoundException">未找到对应用户</exception>
        public void UpdateUserByUserId(long userId, UserInfo user)
        {
            try
            {
                if (userId.GetType() != typeof(long) || user.GetType() != typeof(UserInfo) || userId < 0) throw new ArgumentException();
                _userDao.UpdateUserByUserId(userId, user);
            }catch
            {
                throw;
            }

        }

        public IList<Course> ListCourseByTeacherName(string teacherName)
        {
            throw new NotImplementedException();
        }
    }
}
