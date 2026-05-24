using Microsoft.AspNetCore.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RoyalVilla_API.Models.DTO
{
    public class APIResponse<TData>
    {

        public bool IsSuccess { get; set; }

        public int StatusCode { get; set; }

        public string Message { get; set; } = string.Empty;

        //public object? Data {get; set; }
        public TData? Data { get; set; }
        public object? Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        //Helper Methods 
        //Base one - Abstract one 
        public static APIResponse<TData> Create(bool isSuccess, int statusCode, string message, TData? data = default, object? errors = null)
        {
            return new APIResponse<TData>
            {
                IsSuccess = isSuccess,
                StatusCode = statusCode,
                Message = message,
                Data = data,
                Errors = errors
            };

        }

        //other methods for handle all ststus codes : 


        #region     200 -> Ok
        public static APIResponse<TData> Ok(TData data ,string message) => Create(true, 200, message, data );
        #endregion

        #region     201 -> CreatedAt  
        public static APIResponse<TData> CreatedAt(TData data, string message) => Create(true, 201, message, data);
        #endregion

        #region   204 -> NoContent   
        public static APIResponse<TData> NoContent(string message = "Operation Completed Successfully") => 
            Create(true, 204, message);
        #endregion

        #region      404 -> NotFound
        public static APIResponse<TData> NotFound(string message = "Resource Not Found") =>
            Create(false, 404, message);
        #endregion

        #region    400 -> BadRequest
        public static APIResponse<TData> BadRequest(string message , object? errors = null) =>
            Create(false, 400, message, errors : errors);
        #endregion

        #region     409 -> Conflict
        public static APIResponse<TData> Conflict(string message ) =>
          Create(false, 409, message);
        #endregion

        #region    500 -> InternalServerError        
        public static APIResponse<TData> Error(int statusCode, string message, object? errors = null) =>
            Create(false, statusCode, message, errors : errors);

        #endregion









    }
}
