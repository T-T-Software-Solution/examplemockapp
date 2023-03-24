using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain;

namespace App.Core
{
    public interface IsightingService : IBaseService<Guid, sightingInputModel, sightingViewModel>
    {
        Task<sightingViewModel?> InsertAsync(sightingInputModel model, bool is_force_save);
        Task<sightingViewModel?> UpdateAsync(Guid id, sightingInputModel model, bool is_force_save);
     	Task<List<sightingViewModel>> GetListBySearchAsync(sightingSearchModel model);
		Task<string> UpdateMultipleAsync(List<sightingInputModel> model, bool is_force_save);
        Task<sightingWithSelectionViewModel> GetWithSelectionAsync(Guid id);
        Task<sightingWithSelectionViewModel> GetBlankItemAsync();
		Task RefreshAutoFieldOfAllData();
        Task<sightingEntity?> GetEntityAsync(Guid id);
        Task<byte[]> GetReportStreamAsync(sightingReportRequestModel model);



    }
}

