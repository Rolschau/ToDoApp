using System.Runtime.Serialization;

namespace ToDoWebApi.Models
{
    public class ToDoDTO
    {

        [DataMember(Name = "id")]
        public string? Id { get; set; }

        [DataMember(Name = "value")]
        public string? Value { get; set; }

        [DataMember(Name = "done")]
        public bool Done { get; set; }

        [DataMember(Name = "order")]
        public int? Order { get; set; }
    }
}