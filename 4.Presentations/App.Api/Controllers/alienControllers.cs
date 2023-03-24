using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using App.Api;
using App.Core;
using TTSW.Utils;
using Swashbuckle.AspNetCore.Annotations;
using App.Domain;

namespace App.Controllers
{
    [Route("api/v{version:apiVersion}/alien")]
    [ApiVersion("1.0")]
    [ApiController]
    [Produces("application/json")]
    //[Authorize]
    [SwaggerTag("บริหารจัดการข้อมูล alien")]

    public class alienController : BaseController
    {
        #region Private Variables
        private ILogger<alienController> _logger;
        private IalienService _repository;
		private IConfiguration Configuration { get; set; }
        #endregion

        #region Properties

        #endregion

        /// <summary>
        /// Default constructure for dependency injection
        /// </summary>
        /// <param name="repository"></param>
		/// <param name="configuration"></param>
        /// <param name="logger"></param>
        public alienController(ILogger<alienController> logger, IalienService repository, IConfiguration configuration)
        {
            _logger = logger;
            _repository = repository;
			Configuration = configuration;
        }

        /// <summary>
        /// ดึงข้อมูล alien โดย id
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>ส่งข้อมูล id</returns>
        /// <response code="200">ดึงข้อมูลสำเร็จ</response>
        /// <response code="401">ไม่มีสิทธิในการใช้งาน</response>
        /// <response code="500">มีข้อผิดพลาดในการทำงานจากระบบ</response>  
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(alienWithSelectionViewModel), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var result = await _repository.GetWithSelectionAsync(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception in IActionResult Get.", ex);
                return StatusCode(500, $"{ex.Message}");
            }
        }

		/// <summary>
        /// ดึงรายการเปล่าของ alien สำหรับการสร้างฟอร์มเปล่า
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>ส่งข้อมูลเปล่า</returns>
        /// <response code="200">ดึงข้อมูลสำเร็จ</response>
        /// <response code="401">ไม่มีสิทธิในการใช้งาน</response>
        /// <response code="500">มีข้อผิดพลาดในการทำงานจากระบบ</response>  
        [HttpGet("GetBlankItem")]
        [ProducesResponseType(typeof(alienWithSelectionViewModel), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> GetBlankItem()
        {
            try
            {
                var result = await _repository.GetBlankItemAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception in IActionResult GetBlankItem.", ex);
                return StatusCode(500, $"{ex.Message}");
            }
        }

        /// <summary>
        /// ดึงข้อมูล alien โดยการค้นหา
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>Return list of items by specifced keyword</returns>
        /// <response code="200">ดึงข้อมูลสำเร็จ</response>
        /// <response code="401">ไม่มีสิทธิในการใช้งาน</response>
        /// <response code="500">มีข้อผิดพลาดในการทำงานจากระบบ</response>  
        [HttpGet("GetListBySearch")]
        [ProducesResponseType(typeof(List<alienViewModel>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> GetListBySearch([FromQuery]alienSearchModel model)
        {
            try
            {
                return Ok(await _repository.GetListBySearchAsync(model));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception in IActionResult GetListBySearch.", ex);
                return StatusCode(500, $"{ex.Message}");
            }
        }	

		/// <summary>
        /// ดึงข้อมูลรายงาน alien
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns>Return list of items by specifced keyword</returns>
        /// <response code="200">ดึงข้อมูลสำเร็จ</response>
        /// <response code="401">ไม่มีสิทธิในการใช้งาน</response>
        /// <response code="500">มีข้อผิดพลาดในการทำงานจากระบบ</response>  
        [HttpGet("alien_report")]
        [ProducesResponseType(typeof(FileStreamResult), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> alien_report(alienReportRequestModel model)
        {
            try
            {				
                return File(await _repository.GetReportStreamAsync(model), model.contentType);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while GetReport.", ex);
                return StatusCode(500, $"{ex.Message}");
            }
        }

        /// <summary>
        /// เพิ่มข้อมูล alien
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="model"></param>
        /// <returns>Response Result Message</returns>
        /// <response code="200">ดำเนินการเรียบร้อย</response>
        /// <response code="401">ไม่มีสิทธิในการใช้งาน</response>
        /// <response code="500">มีข้อผิดพลาดในการทำงานจากระบบ</response>  
        [HttpPost("")]
        [ProducesResponseType(typeof(CommonResponseMessage), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Insert([FromBody] alienInputModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _repository.InsertAsync(model, true);
					var message = new CommonResponseMessage();
                    message.code = "200";
                    message.message = $"เพิ่มข้อมูล เรียบร้อย";
                    message.data = result;
                    return Ok(message);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical($"Exception while insert.", ex);
                    return StatusCode(500, $"{ex.Message}");
                }
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// ปรับปรุงข้อมูล alien
        /// </summary>
        /// <remarks>
        /// </remarks>
		/// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns>Response Result Message</returns>
        /// <response code="200">ดำเนินการเรียบร้อย</response>
        /// <response code="401">ไม่มีสิทธิในการใช้งาน</response>
        /// <response code="500">มีข้อผิดพลาดในการทำงานจากระบบ</response>  
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CommonResponseMessage), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Guid id, [FromBody] alienInputModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _repository.UpdateAsync(id, model, true);
					var message = new CommonResponseMessage();
                    message.code = "200";
                    message.message = $"แก้ไขข้อมูล เรียบร้อย";
                    message.data = result;
                    return Ok(message);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical($"Exception while update {id.ToString()}.", ex);
                    return StatusCode(500, $"{id.ToString()}. {ex.Message}");
                }
            }

            return BadRequest(ModelState);
        }        

        /// <summary>
        /// ลบรายการ alien
        /// </summary>
        /// <remarks>
        /// </remarks>
		/// <param name="id"></param>
        /// <returns>Response Result Message</returns>
        /// <response code="200">ดำเนินการเรียบร้อย</response>
        /// <response code="401">ไม่มีสิทธิในการใช้งาน</response>
        /// <response code="500">มีข้อผิดพลาดในการทำงานจากระบบ</response>  
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(CommonResponseMessage), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _repository.DeleteAsync(id);
					var message = new CommonResponseMessage();
                    message.code = "200";
                    message.message = $"ลบข้อมูล เรียบร้อย";
                    message.data = null;
                    return Ok(message);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical($"Exception while delete {id}.", ex);
                    return StatusCode(500, $"{id}. {ex.Message}");
                }
            }

            return BadRequest(ModelState);
        }

		/// <summary>
        /// ปรับปรุง alien ข้อมูลทีละหลายรายการ
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="model"></param>
        /// <returns>Response Result Message</returns>
        /// <response code="200">ดำเนินการเรียบร้อย</response>
        /// <response code="401">ไม่มีสิทธิในการใช้งาน</response>
        /// <response code="500">มีข้อผิดพลาดในการทำงานจากระบบ</response>  
        [HttpPut("UpdateMultiple")]
        [ProducesResponseType(typeof(CommonResponseMessage), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMultiple([FromBody] List<alienInputModel> model)
        {
            if (ModelState.IsValid)
            {
				try
				{
					string rowCount = await _repository.UpdateMultipleAsync(model, true);
					var message = new CommonResponseMessage();
                    message.code = "200";
                    message.message = "ปรับปรุงข้อมูลเรียบร้อย จำนวน "+rowCount+" รายการ";
                    message.data = null;
                    return Ok(message);
				}
				catch (Exception ex)
                {
                    _logger.LogCritical($"Exception while UpdateMultiple.", ex);
                    return StatusCode(500, $"{ex.Message}");
                }   
            }

            return BadRequest(ModelState);
        }



    }
}
