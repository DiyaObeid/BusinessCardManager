// ResultDto.cs defines a data transfer object (DTO) that encapsulates 
// the result of an operation within the Business Card Manager application. 
// It contains properties to indicate the success of the operation and 
// an optional message providing additional context or information.

namespace BusinessCardManager.Core.DTOs
{
    public class ResultDto
    {
        // Indicates whether the operation was successful
        public bool Succeeded { get; set; }

        // Optional message providing additional information about the operation
        public string? Message { get; set; }
    }
}
