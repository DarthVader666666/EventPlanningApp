using AutoMapper;
using EventPlanning.Bll.Interfaces;
using EventPlanning.Data.Entities;
using EventPlanning.Api.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanning.Api.Controllers
{
    [EnableCors("AllowClient")]
    [ApiController]    
    public class ThemesController : ControllerBase
    {
        private readonly ILogger<ThemesController> _logger;
        private readonly IRepository<Theme> _themeRepository;
        private readonly IMapper _mapper;

        public ThemesController(IMapper mapper, IRepository<Theme> themeRepository, ILogger<ThemesController> logger)
        {
            _logger = logger;
            _themeRepository = themeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("api/[controller]")]
        public async Task<IEnumerable<ThemeIndexModel>> Index()
        {
            var themes = await _themeRepository.GetListAsync();
            var mappedThemes = _mapper.Map<IEnumerable<Theme>, IEnumerable<ThemeIndexModel>>(themes.ToList());
            return mappedThemes;
        }
    }
}
