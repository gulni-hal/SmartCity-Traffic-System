using System.Threading.Tasks;
using Auth.Application.Entities;

namespace Auth.Application.Interfaces;

public interface IUserRepository
{
    //Ayni kullanici adindan var mi diye kontrol etmek icin
   Task<User?> GetByUsernameAsync(string username);

    //Yeni kullaniciyi veritabanina kaydetmek icin
   Task CreateAsync(User user);
}