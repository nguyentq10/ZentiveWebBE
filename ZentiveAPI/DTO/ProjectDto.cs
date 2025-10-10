namespace ZentiveAPI.DTO
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; }
        public decimal Goal { get; set; }
        public decimal CurrentAmount { get; set; }
        public string MediaCoverUrl { get; set; }
        public string Status { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }

        public string CategoryName { get; set; }
        public string CreatorName { get; set; }
    }
}
