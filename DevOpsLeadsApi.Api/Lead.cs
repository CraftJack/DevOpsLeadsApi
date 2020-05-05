using System;

namespace DevOpsLeadsApi.Api
{
    public class Lead
    {
        private readonly string _firstName;
        private readonly string _lastName;

        public Lead(string firstName, string lastName) : this(null, firstName, lastName){}
        
        public Lead(int? id, string firstName, string lastName)
        {
            Id = id;
            _firstName = firstName;
            _lastName = lastName;
        }
        public int? Id { get; set; }
        
        public string FullName => $"{_firstName} {_lastName}";

        public string Address { get; set; }
        public string Service { get; set; }
        
        public DateTime CreatedAt { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Lead other))
            {
                return false;
            }
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}