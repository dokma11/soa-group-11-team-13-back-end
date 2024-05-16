using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Dtos
{
    public class CommentPagedResponseDto
    {
        public List<CommentResponseDto> Comments { get; set; }
        public int TotalCount { get; set; }
    }
}
