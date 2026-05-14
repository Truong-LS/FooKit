using System;

namespace MyProject.Domain.Entities
{
    public class UserTool
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string ToolName { get; set; } = string.Empty;

        public virtual User? User { get; set; }
    }
}
