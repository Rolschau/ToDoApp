using System.Runtime.Serialization;

namespace ToDoWebApi.BLL.Models
{
    public class ToDoDTO : ICloneable
    {

        [DataMember(Name = "id")]
        public string? Id { get; set; }

        [DataMember(Name = "value")]
        public string? Value { get; set; }

        [DataMember(Name = "done")]
        public bool Done { get; set; }

        [DataMember(Name = "order")]
        public int Order { get; set; }

        public object Clone() {
            return (ToDoDTO)MemberwiseClone();
        }
    }
}