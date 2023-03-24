using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain;
using System.IO;
using System.Web;
using System.Net;
using Microsoft.Extensions.Options;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using App.Core;

namespace App.Database
{
    public class alienService : IalienService
    {
        private IBaseRepository<alienEntity, Guid> _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly DataContext _dataContext;

        public alienService(
            IBaseRepository<alienEntity, Guid> repository, 
            IMapper mapper, 
            IConfiguration configuration,
            DataContext dataContext
            )
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
            _dataContext = dataContext;
        }

        #region Private Functions
        private alienEntity GetEntity(alienInputModel model)
        {
            return _mapper.Map<alienEntity>(model);
        }
        private alienViewModel GetDto(alienEntity entity)
        {
            return _mapper.Map<alienViewModel>(entity);
        }
       
        #endregion

        #region Public Functions
        #region Query Functions

        public async Task<alienViewModel?> GetAsync(Guid id)
        {
            var entity = await _repository.GetAsync(id);

            return GetDto(entity);
        }

        public async Task<alienEntity?> GetEntityAsync(Guid id)
        {
            var entity = await _repository.GetAsync(id);

            return entity;
        }

        public async Task<alienWithSelectionViewModel> GetWithSelectionAsync(Guid id)
        {
            var entity = await _repository.GetAsync(id);
            var i = _mapper.Map<alienWithSelectionViewModel>(entity);


            return i;
        }

        public async Task<alienWithSelectionViewModel> GetBlankItemAsync()
        {
            var i = new alienWithSelectionViewModel();


            return i;
        }

        public async Task<List<alienViewModel>> GetListBySearchAsync(alienSearchModel model)
        {
            var data = await (
                from m_alien in _dataContext.aliens


				where
                1 == 1 
                && (string.IsNullOrEmpty(model.name) || m_alien.name.Contains(model.name))
                && (string.IsNullOrEmpty(model.species) || m_alien.species.Contains(model.species))
                && (string.IsNullOrEmpty(model.origin_planet) || m_alien.origin_planet.Contains(model.origin_planet))


                orderby m_alien.created descending
                select new alienViewModel()
                {
                    id = m_alien.id,
                    name = m_alien.name,
                    species = m_alien.species,
                    origin_planet = m_alien.origin_planet,


                    isActive = m_alien.isActive,
                    created = m_alien.created,
                    updated = m_alien.updated
                }
            ).Take(1000).ToListAsync();

            return data;
        }

        #endregion        

        #region Manipulation Functions



        public async Task<alienViewModel?> InsertAsync(alienInputModel model, bool is_force_save)
        {
            var entity = GetEntity(model);
            entity.id = Guid.NewGuid();


            
            if (is_force_save)
            {
                var inserted = await _repository.InsertAsync(entity);
                return await GetAsync(inserted.id);
            }
            else
            {
                await _repository.InsertWithoutCommitAsync(entity);
                return _mapper.Map<alienViewModel>(entity);
            }
        }

        public async Task<alienViewModel?> UpdateAsync(Guid id, alienInputModel model, bool is_force_save)
        {
            var existingEntity = await _repository.GetAsync(id);
            if (existingEntity != null)
            {
                existingEntity.name = model.name;
                existingEntity.species = model.species;
                existingEntity.origin_planet = model.origin_planet;


                if (is_force_save)
                {
                    var updated = await _repository.UpdateAsync(id, existingEntity);
                    return await GetAsync(updated.id);
                }
                else
                {
                    await _repository.UpdateWithoutCommitAsync(id, existingEntity);
                    return _mapper.Map<alienViewModel>(existingEntity);
                }
            }
            else
            throw new Exception("No data to update");
        }

		public async Task<string> UpdateMultipleAsync(List<alienInputModel> model, bool is_force_save)
        {
            foreach(var i in model)
            {
                if (i.active_mode == "1" && i.id.HasValue) // update
                {                    
                    var existingEntity = await _repository.GetAsync(i.id.Value);
                    if (existingEntity != null)
                    {
                existingEntity.name = i.name;
                existingEntity.species = i.species;
                existingEntity.origin_planet = i.origin_planet;


                        await _repository.UpdateWithoutCommitAsync(i.id.Value, existingEntity);
                    }
                }
                else if (i.active_mode == "1" && !i.id.HasValue) // add
                {
                    var entity = GetEntity(i);
                    entity.id = Guid.NewGuid();
                    await _repository.InsertWithoutCommitAsync(entity);
                }
                else if (i.active_mode == "0" && i.id.HasValue) // remove
                {                    
                    await _repository.DeleteWithoutCommitAsync(i.id.Value);
                }
                else if (i.active_mode == "0" && !i.id.HasValue)
                {
                    // nothing to do
                }                
            }
            if (is_force_save)
            {
                await _dataContext.SaveChangesAsync();
            } 

            return model.Count().ToString();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);

            return;
        }

		public async Task RefreshAutoFieldOfAllData()
        {
            var all_items = _dataContext.aliens;
            foreach (var item in all_items)
            {
                
            }
            await _dataContext.SaveChangesAsync();
        }

        #endregion

        #region Match Item

        #endregion

        #endregion

        public async Task<Stream> GetReportStreamAsync(alienReportRequestModel model)
        {
            using (var httpclient = new HttpClient())
            {
                string mainurl = MyHelper.GetConfig(_configuration, "JasperReportServer:MainURL");
                string reportsite = MyHelper.GetConfig(_configuration, "JasperReportServer:reportsite");
                string username = MyHelper.GetConfig(_configuration, "JasperReportServer:username");
                string password = MyHelper.GetConfig(_configuration, "JasperReportServer:password");

                string url = $"{mainurl}{reportsite}/xxใส่ชื่อรายงานตรงนี้xx.{model.filetype}?{MyHelper.GetParameterForJasperReport(model)}&j_username={username}&j_password={password}";

                if (model.filetype == "xlsx")
                {
                    url += "&ignorePagination=true";
                }

                using (var data = await httpclient.GetAsync(url))
                using (var content = data.Content)
                {
                    return await content.ReadAsStreamAsync();
                }
                
            }
        }

    }
}