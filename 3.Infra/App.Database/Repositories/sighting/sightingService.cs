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
    public class sightingService : IsightingService
    {
        private IBaseRepository<sightingEntity, Guid> _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly DataContext _dataContext;

        public sightingService(
            IBaseRepository<sightingEntity, Guid> repository, 
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
        private sightingEntity GetEntity(sightingInputModel model)
        {
            return _mapper.Map<sightingEntity>(model);
        }
        private sightingViewModel GetDto(sightingEntity entity)
        {
            return _mapper.Map<sightingViewModel>(entity);
        }
       
        #endregion

        #region Public Functions
        #region Query Functions

        public async Task<sightingViewModel?> GetAsync(Guid id)
        {
            var entity = await _repository.GetAsync(id);

            return GetDto(entity);
        }

        public async Task<sightingEntity?> GetEntityAsync(Guid id)
        {
            var entity = await _repository.GetAsync(id);

            return entity;
        }

        public async Task<sightingWithSelectionViewModel> GetWithSelectionAsync(Guid id)
        {
            var entity = await _repository.GetAsync(id);
            var i = _mapper.Map<sightingWithSelectionViewModel>(entity);
            i.item_alien_id = await _dataContext.aliens.Select(x => _mapper.Map<alienViewModel>(x)).ToListAsync();


            return i;
        }

        public async Task<sightingWithSelectionViewModel> GetBlankItemAsync()
        {
            var i = new sightingWithSelectionViewModel();
            i.item_alien_id = await _dataContext.aliens.Select(x => _mapper.Map<alienViewModel>(x)).ToListAsync();


            return i;
        }

        public async Task<List<sightingViewModel>> GetListBySearchAsync(sightingSearchModel model)
        {
            var data = await (
                from m_sighting in _dataContext.sightings

                join fk_alien1 in _dataContext.aliens on m_sighting.alien_id equals fk_alien1.id
                into alienResult1
                from fk_alienResult1 in alienResult1.DefaultIfEmpty()


				where
                1 == 1 
                && (!model.alien_id.HasValue || m_sighting.alien_id == model.alien_id)
                && (!model.found_date.HasValue || m_sighting.found_date == model.found_date)
                && (string.IsNullOrEmpty(model.location) || m_sighting.location.Contains(model.location))
                && (string.IsNullOrEmpty(model.witness) || m_sighting.witness.Contains(model.witness))


                orderby m_sighting.created descending
                select new sightingViewModel()
                {
                    id = m_sighting.id,
                    alien_id = m_sighting.alien_id,
                    found_date = m_sighting.found_date,
                    location = m_sighting.location,
                    witness = m_sighting.witness,

                    alien_id_alien_species = fk_alienResult1.species,

                    isActive = m_sighting.isActive,
                    created = m_sighting.created,
                    updated = m_sighting.updated
                }
            ).Take(1000).ToListAsync();

            return data;
        }

        #endregion        

        #region Manipulation Functions



        public async Task<sightingViewModel?> InsertAsync(sightingInputModel model, bool is_force_save)
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
                return _mapper.Map<sightingViewModel>(entity);
            }
        }

        public async Task<sightingViewModel?> UpdateAsync(Guid id, sightingInputModel model, bool is_force_save)
        {
            var existingEntity = await _repository.GetAsync(id);
            if (existingEntity != null)
            {
                existingEntity.alien_id = model.alien_id;
                existingEntity.found_date = model.found_date;
                existingEntity.location = model.location;
                existingEntity.witness = model.witness;


                if (is_force_save)
                {
                    var updated = await _repository.UpdateAsync(id, existingEntity);
                    return await GetAsync(updated.id);
                }
                else
                {
                    await _repository.UpdateWithoutCommitAsync(id, existingEntity);
                    return _mapper.Map<sightingViewModel>(existingEntity);
                }
            }
            else
            throw new Exception("No data to update");
        }

		public async Task<string> UpdateMultipleAsync(List<sightingInputModel> model, bool is_force_save)
        {
            foreach(var i in model)
            {
                if (i.active_mode == "1" && i.id.HasValue) // update
                {                    
                    var existingEntity = await _repository.GetAsync(i.id.Value);
                    if (existingEntity != null)
                    {
                existingEntity.alien_id = i.alien_id;
                existingEntity.found_date = i.found_date;
                existingEntity.location = i.location;
                existingEntity.witness = i.witness;


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
            var all_items = _dataContext.sightings;
            foreach (var item in all_items)
            {
                
            }
            await _dataContext.SaveChangesAsync();
        }

        #endregion

        #region Match Item

        #endregion

        #endregion

        public async Task<byte[]> GetReportStreamAsync(sightingReportRequestModel model)
        {
            using (var httpclient = new HttpClient())
            {
                string mainurl = MyHelper.GetConfig(_configuration, "JasperReportServer:MainURL");
                string reportsite = MyHelper.GetConfig(_configuration, "JasperReportServer:reportsite");
                string username = MyHelper.GetConfig(_configuration, "JasperReportServer:username");
                string password = MyHelper.GetConfig(_configuration, "JasperReportServer:password");

                string url = $"{mainurl}{reportsite}/alien01.{model.filetype}?{MyHelper.GetParameterForJasperReport(model)}&j_username={username}&j_password={password}";

                if (model.filetype == "xlsx")
                {
                    url += "&ignorePagination=true";
                }

                using (var data = await httpclient.GetAsync(url))
                using (var content = data.Content)
                {
                    return ReadFully(await content.ReadAsStreamAsync());
                }
                
            }
        }

        public byte[] ReadFully(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

    }
}