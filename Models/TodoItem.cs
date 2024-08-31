using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
    public class TodoItem
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("iscomplete")]
        public bool IsComplete { get; set; }
    }
}