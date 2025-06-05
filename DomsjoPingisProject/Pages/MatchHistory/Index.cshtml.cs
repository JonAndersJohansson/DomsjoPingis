using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DomsjoPingisProject.ViewModels;
using Service.Interface;

namespace DomsjoPingisProject.Pages.MatchHistory
{
    [Authorize(Roles = "Admin")]

    public class IndexModel : PageModel
    {
        private readonly IMatchHistoryService _matchServices;
        private readonly IMapper _mapper;

        public IndexModel(IMatchHistoryService matchServices,
            IMapper mapper)
        {
            _matchServices = matchServices;
            _mapper = mapper;
        }

        public List<MatchViewModel> Matches { get; set; } = new();

        public string Q { get; set; }
        public int CurrentPage { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public int PageCount { get; set; }

        public void OnGet(string sortColumn, string sortOrder,
            string q, int pageNo)
        {
            Q = q;
            SortColumn = sortColumn;
            SortOrder = sortOrder;

            if (pageNo == 0)
                pageNo = 1;
            CurrentPage = pageNo;

            var result = _matchServices.ReadMatches
                (sortColumn, sortOrder, q, pageNo);

            PageCount = result.PageCount;

            Matches = _mapper.Map<List<MatchViewModel>>(result.Results);
        }
    }
}