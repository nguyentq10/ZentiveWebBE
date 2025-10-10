namespace Service.Response
    {
        public class ProjectResponse
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Summary { get; set; }
            public decimal Goal { get; set; }
            public decimal CurrentAmount { get; set; }
            public string MediaCoverUrl { get; set; }
            public DateTime? StartAt { get; set; }
            public DateTime? EndAt { get; set; }
            public string Status { get; set; }
            public DateTime CreatedAt { get; set; }

            public string CreatorName { get; set; }
            public string CreatorEmail { get; set; }
            public string CreatorPhone { get; set; }

            public List<RewardTierResponse> RewardTiers { get; set; } = new();
        }

        public class RewardTierResponse
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
            public int Quantity { get; set; }
            public DateTime? EstimatedDelivery { get; set; }
        }
}
