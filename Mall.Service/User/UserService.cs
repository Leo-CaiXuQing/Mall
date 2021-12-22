using Mall.IService.User;
using Mall.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mall.Service.User
{
    public class UserService : IUserService
    {
        private readonly List<UserInfo> users = new List<UserInfo>()
        {
            new UserInfo(){ Id=1, UserName="Leo", Password="123",Roles=new string[]{"管理员"} },
            new UserInfo(){ Id=2, UserName="林北", Password="123",Roles=new string[]{"会员"}},
            new UserInfo(){ Id=3, UserName="简", Password="123",Roles=new string[]{"会员"}},
            new UserInfo(){ Id=4, UserName="药家鑫", Password="123",Roles=new string[]{"会员"}},
            new UserInfo(){ Id=5, UserName="马加爵", Password="123",Roles=new string[]{"会员"}},
            new UserInfo(){ Id=6, UserName="韩信", Password="123",Roles=new string[]{"会员"}},
            new UserInfo(){ Id=7, UserName="薛仁贵", Password="123",Roles=new string[]{"会员"}},
            new UserInfo(){ Id=8, UserName="娜扎", Password="123",Roles=new string[]{"会员"}},
            new UserInfo(){ Id=9, UserName="迪丽热巴", Password="123",Roles=new string[]{"会员"}},
            new UserInfo(){ Id=10, UserName="王昭君", Password="123",Roles=new string[]{"会员"}},
            new UserInfo(){ Id=11, UserName="张飞", Password="123",Roles=new string[]{"会员"}},
            new UserInfo(){ Id=12, UserName="陈翔", Password="123",Roles=new string[]{"会员"} }
        };
        public UserInfo FindUser(int Id)
        {
            return users.Find(t => t.Id == Id);
        }

        public UserInfo FindUserByName(string Name, string Password)
        {
            return users.Find(t => t.UserName == Name && t.Password == Password);
        }

        public UserInfo FindUserByName(string Name)
        {
            return users.Where(t => t.UserName == Name).FirstOrDefault();
        }

        public IEnumerable<UserInfo> UserAll()
        {
            return users;
        }
    }
}
