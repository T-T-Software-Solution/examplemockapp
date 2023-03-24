using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Core
{
    public interface IBaseService<Key, InputModel, ViewModel>
    {
        #region Query Functions
        Task<ViewModel?> GetAsync(Key id);
        #endregion

        #region Manipulation Functions
        Task<ViewModel?> InsertAsync(InputModel model, bool is_force_save);
        Task<ViewModel?> UpdateAsync(Key id, InputModel model, bool is_force_save);
        Task DeleteAsync(Key id);
        #endregion
    }
}
