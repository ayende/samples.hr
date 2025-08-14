using System.ComponentModel.DataAnnotations;

namespace HRAssistant.Models
{
    public class SignatureDocument
    {
        public string Id { get; set; } = string.Empty;
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Type { get; set; } = string.Empty; // "NDA", "Policy", "Agreement", etc.
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        public int Version { get; set; } = 1;
        
        [Required]
        public DateTime CreatedDate { get; set; }
        
        [Required]
        public string CreatedBy { get; set; } = string.Empty;
        
        public DateTime? LastUpdated { get; set; }
        
        public string? UpdatedBy { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public bool RequiresSignature { get; set; } = true;
        
        public List<string> Tags { get; set; } = new();
        
        public string? Description { get; set; }
        
        public int? ExpirationDays { get; set; } // Days until signature expires (null = never expires)
    }

    public class SignedDocument
    {
        [Required]
        public string DocumentId { get; set; } = string.Empty;
        
        [Required]
        public string DocumentTitle { get; set; } = string.Empty;
        
        [Required]
        public int DocumentVersion { get; set; }
        
        [Required]
        public DateTime SignedDate { get; set; }
        
        public string? SignatureAttachmentName { get; set; } // File name of the signature image
        
        [Required]
        public string SignedBy { get; set; } = string.Empty; // Employee name
        
        public string? SignatureMethod { get; set; } = "Digital"; // "Digital", "Physical", etc.
        
        public DateTime? ExpirationDate { get; set; }
        
        public string? Notes { get; set; }
    }
}
