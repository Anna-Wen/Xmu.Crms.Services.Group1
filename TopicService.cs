/*************/
/*************/
/*@author 1-4*/
/*************/
/*************/
using System;
using System.Collections.Generic;
using System.Text;
using Xmu.Crms.Services.Group1.Dao;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Services.Group1
{
    class TopicService : ITopicService
    {
        /// <summary>
        /// @author Group 1-4
        /// @version 2.00
        /// </summary>
        private readonly ITopicDao _topicDao;

        public TopicService(ITopicDao topicDao)
        {
            _topicDao = topicDao;
        }

        /// <summary>
        /// 按topicId删除SeminarGroupTopic表信息.
        /// @author Group 1-4
        /// </summary>
        /// <param name="topicId">讨论课Id</param>
        /// <exception cref="T:System.ArgumentException">topicId格式错误</exception>
        public void DeleteSeminarGroupTopicByTopicId(long topicId)
        {
            _topicDao.DeleteSeminarGroupTopicByTopicId(topicId);
        }

        /// <summary>
        /// 小组取消选择话题.
        /// @author Group 1-4
        /// 删除seminar_group_topic表的记录
        /// </summary>
        /// <param name="groupId">小组Id</param>
        /// <param name="topicId">话题Id</param>
        /// <exception cref="T:System.ArgumentException">groupId格式错误或topicId格式错误时抛出</exception>
        public void DeleteSeminarGroupTopicById(long groupId, long topicId)
        {
            _topicDao.DeleteSeminarGroupTopic(topicId,groupId);    
        }

        // <summary>
        /// 按seminarId删除话题.
        /// @author Group 1-4
        /// 根据seminarId获得topic信息，然后再根据topic删除seninargrouptopic信息和StudentScoreGroup信息，最后再根据删除topic信息
        /// </summary>
        /// <param name="seminarId">讨论课Id</param>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ITopicService.ListTopicBySeminarId(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ITopicService.DeleteSeminarGroupTopicByTopicId(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IGradeService.DeleteStudentScoreGroupByTopicId(System.Int64)"/>
        /// <exception cref="T:System.ArgumentException">seminarId格式错误</exception>
        public void DeleteTopicBySeminarId(long seminarId)
        { 
            IList<Topic> tidlist = _topicDao.List(seminarId);
           foreach(Topic t in tidlist)
            {
                _topicDao.DeleteSeminarGroupTopicByTopicId(t.Id);
                _topicDao.DeleteTopic(t.Id);
            }
        }

        /// <summary>
        /// 删除topic.
        /// </summary>
        /// <param name="topicId">要删除的topic的topicId</param>       
        /// <exception cref="T:System.ArgumentException">Id格式错误时抛出</exception>
        public void DeleteTopicByTopicId(long topicId)
        {
            _topicDao.DeleteSeminarGroupTopicByTopicId(topicId);
            _topicDao.DeleteTopic(topicId);
        }

        ///<summary>
        ///按话题id和小组id获取讨论课小组选题信息
        ///</summary>
        ///<param name="topicId">讨论课Id</param>
        ///<param name="groupId">小组Id</param>
        /// <returns>seminarGroupTopic 讨论课小组选题信息</returns>
        ///  <exception cref="T:System.ArgumentException">seminarId格式错误</exception>
        public SeminarGroupTopic GetSeminarGroupTopicById(long topicId, long groupId)
        {
            try
            {
                return _topicDao.GetSeminarGroupTopic(topicId, groupId);
            }catch(TopicNotFoundException e) { throw e; }
            
        }

        /// <summary>
        /// 按topicId获取topic.
        /// @author Group 1-4
        /// </summary>
        /// <param name="topicId">要获取的topic的topicId</param>
        /// <returns>该topic</returns>
        /// <exception cref="T:System.ArgumentException">Id格式错误时抛出</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.TopicNotFoundException">无此小组或Id错误</exception>
        public Topic GetTopicByTopicId(long topicId)
        {
            try
            {
                return _topicDao.GetTopic(topicId);
            }catch(TopicNotFoundException e) { throw e; }

        }

        /// <summary>
        /// 根据讨论课Id和topic信息创建一个话题.
        /// @author Group 1-4
        /// </summary>
        /// <param name="seminarId">话题所属讨论课的Id</param>
        /// <param name="topic">话题</param>
        /// <returns>新建话题后给topic分配的Id</returns>
        /// <exception cref="T:System.ArgumentException">Id格式错误时抛出</exception>
        public long InsertTopicBySeminarId(long seminarId, Topic topic)
        {
            Seminar s = new Seminar();
            long result;
            try
            {
                s = _topicDao.FindSeminar(seminarId);  //该门讨论课存在
                topic.Seminar = s;
                result = _topicDao.Insert(seminarId, topic);
            }catch(SeminarNotFoundException e) { throw e; }
            return result;
        }

        /// <summary>
        /// 按seminarId获取Topic.
        /// @author Group 1-4
        /// </summary>
        /// <param name="seminarId">课程Id</param>
        /// <returns>null</returns>
        /// <exception cref="T:System.ArgumentException">Id格式错误时抛出</exception>
        public IList<Topic> ListTopicBySeminarId(long seminarId)
        {
            try
            {
                return _topicDao.List(seminarId);
            }catch (TopicNotFoundException e) { throw e; }
        }

        /// <summary>
        /// 根据topicId修改topic.
        /// @author Group 1-4
        /// </summary>
        /// <param name="topicId">讨论课的ID</param>
        /// <param name="topic">修改后的讨论课</param>
        /// <exception cref="T:System.ArgumentException">Id格式错误时抛出</exception>
        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.TopicNotFoundException">无此小组或Id错误</exception>
        public void UpdateTopicByTopicId(long topicId, Topic topic)
        {
            try
            {
                _topicDao.Update(topicId, topic);
            }catch(TopicNotFoundException e) { throw e; }
        }

        /// 根据小组id获取该小组该堂讨论课所有选题信息
        /// <p>根据小组id获取该小组该堂讨论课所有选题信息<br>
        /// @param groupId
        /// @return list 该小组该堂讨论课选题列表
        /// @exception IllegalArgumentException groupId格式错误
        public List<SeminarGroupTopic> ListSeminarGroupTopicByGroupId(long groupId)
        {
            try
            {
                return _topicDao.FindSeminarGroupTopicByGroupId(groupId);
            }
            catch(SeminarNotFoundException e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 查询剩余话题
        /// </summary>
        /// <param name="topicId">话题id</param>
        /// <param name="classId">班级id</param>
        /// <returns>topicNum 剩余话题数量</returns>
        public int GetRestTopicById(long topicId,long classId)
        {
            int result = 0;
            int count=0;
            try
            {
                Topic topic = _topicDao.GetTopic(topicId);
                result = topic.GroupNumberLimit;
                IList<SeminarGroup> seminarGroup = _topicDao.GetSeminarGroupById(classId, topic.Seminar.Id);
                foreach (var s in seminarGroup)
                {
                    SeminarGroupTopic seminarGroupTopic = _topicDao.GetSeminarGroupTopic(topicId, s.Id);
                    if(seminarGroupTopic!=null)
                           count++;
                }
            }
            catch(System.Exception e)
            {
                if (e.ToString().Equals("找不到该话题!"))
                    throw e;
            }
            result -= count;
            return result;          
        }

    }
}
