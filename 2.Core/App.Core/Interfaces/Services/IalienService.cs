using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Domain;

namespace App.Core
{
    public interface IalienService : IBaseService<Guid, alienInputModel, alienViewModel>
    {
        Task<alienViewModel?> InsertAsync(alienInputModel model, bool is_force_save);
        Task<alienViewModel?> UpdateAsync(Guid id, alienInputModel model, bool is_force_save);
     	Task<List<alienViewModel>> GetListBySearchAsync(alienSearchModel model);
		Task<string> UpdateMultipleAsync(List<alienInputModel> model, bool is_force_save);
        Task<alienWithSelectionViewModel> GetWithSelectionAsync(Guid id);
        Task<alienWithSelectionViewModel> GetBlankItemAsync();
		Task RefreshAutoFieldOfAllData();
        Task<alienEntity?> GetEntityAsync(Guid id);
        Task<Stream> GetReportStreamAsync(alienReportRequestModel model);



    }
}

