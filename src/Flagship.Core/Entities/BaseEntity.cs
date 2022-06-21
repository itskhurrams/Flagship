namespace Flagship.Core.Entities {
    public class BaseEntity {
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public TimeSpan CreatedTime { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public TimeSpan? UpdatedTime { get; set; }
    }
}
