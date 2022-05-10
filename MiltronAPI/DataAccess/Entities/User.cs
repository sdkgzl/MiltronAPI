using DataAccess.Base;
using System.Text.Json.Serialization;


namespace DataAccess.Entities
{
    public class User : BaseEntity, IBaseEntity
    {
        public User()
        {
            InverseAddedByUser = new HashSet<User>();
            InverseLastModifiedByUser = new HashSet<User>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }
        [JsonIgnore]
        public string PasswordSalt { get; set; }       
      
        public virtual User AddedByUser { get; set; }
        public virtual User LastModifiedByUser { get; set; }
        public virtual ICollection<User> InverseAddedByUser { get; set; }
        public virtual ICollection<User> InverseLastModifiedByUser { get; set; }
    }
}
