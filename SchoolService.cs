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

namespace Xmu.Crms.Services.Group1
{
    class SchoolService : ISchoolService
    {
        /// <summary>
        /// @author Group Group1
        /// @version 2.00
        /// </summary>
        private readonly ISchoolDao _schoolDao;
        public SchoolService(ISchoolDao schoolDao)
        {
            _schoolDao = schoolDao;
        }
        /// <summary>
        /// 获取学校信息.
        /// @author Group Group1
        /// </summary>
        /// <param name="schoolId">学校id</param>
        /// <returns>SchoolBO 学校信息</returns>
        public School GetSchoolBySchoolId(long schoolId)
        {
            School school = _schoolDao.Find(schoolId);
            return school;
        }

        /// <summary>
        /// 添加学校.
        /// @author Group Group1
        /// </summary>
        /// <param name="school">学校的信息</param>
        /// <returns>schoolId 学校的id</returns>
        public long InsertSchool(School school)
        {
            return _schoolDao.AddSchool(school);
        }

        /// <summary>
        /// 获取城市列表.
        /// @author Group Group1
        /// </summary>
        /// <param name="province">省份名称</param>
        /// <returns>list 城市名称列表</returns>
        public IList<string> ListCity(string province)
        {
            IList<string> city=new List<string>();
            List<School> school = _schoolDao.FindAllByProvince(province);
            foreach(School s in school)
            {
                if (!city.Contains(s.City))
                    city.Add(s.City);
            }
            return city;
        }

        /// <summary>
        /// 获取省份列表.
        /// @author Group Group1
        /// </summary>
        /// <returns>list 省份名称列表</returns>
        public IList<string> ListProvince()
        {
            IList<School> school = _schoolDao.FindAll();
            IList<string> province=new List<string>();
            foreach(School s in school)
            {
                if (!province.Contains(s.Province))
                    province.Add(s.Province);
            }
            return province;
        }

        /// <summary>
        /// 按城市名称查学校.
        /// @author Group Group1
        /// </summary>
        /// <param name="city">城市名称</param>
        /// <returns>list 学校列表</returns>
        public IList<School> ListSchoolByCity(string city)
        {
            IList<School> list = _schoolDao.FindAllByCity(city);
            return list;
        }

        //public School getSchoolbySchoolName(string name)
        //{
        //    School school = _schoolDao.getSchoolBySchoolName(name);
        //    return school;
        //}
    }
}
