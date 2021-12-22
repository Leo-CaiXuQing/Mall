using Mall.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mall.IService.User
{
    public interface IUserService
    {
        /// <summary>
        /// 根据用户Id进行查询
        /// </summary>
        /// <param name="Id">用户Id</param>
        /// <returns></returns>
        UserInfo FindUser(int Id);
     
        /// <summary>
        /// 查询所有用户资料
        /// </summary>
        /// <returns></returns>
        IEnumerable<UserInfo> UserAll();

        /// <summary>
        /// 根据用户名查询资料
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        UserInfo FindUserByName(string Name);

        /// <summary>
        /// 根据用户名和密码查询资料
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        UserInfo FindUserByName(string Name, string Password);
    }
}
