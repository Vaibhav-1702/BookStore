using BusinessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Model.Model;
using Model.Utility;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookBL _bookBL;

        public BookController(IBookBL bookBL)
        {
            _bookBL = bookBL;
        }

        [HttpPost("AddBook")]
        public async Task<ResponseModel<Book>> AddBook(BookDto bookDto)
        {
            return await _bookBL.AddBook(bookDto);
        }

        [HttpDelete("DeleteBook")]
        public async Task<ResponseModel<bool>> DeleteBookById(int bookId)
        {
            return await _bookBL.DeleteBookById(bookId);
        }

        [HttpGet("GetAllBook")]
        public async Task<ResponseModel<IEnumerable<Book>>> GetAllBooks()
        {
            return await _bookBL.GetAllBooks();
        }

        [HttpGet("GetBookById")]
        public async Task<ResponseModel<Book>> GetBookById(int bookId)
        {
            return await _bookBL.GetBookById(bookId);
        }

        [HttpPut("UpdateBook")]
        public async Task<ResponseModel<Book>> UpdateBook(int bookId, UpdateBookDto updateBookDto)
        {
            return await _bookBL.UpdateBook(bookId, updateBookDto);
        }
    }
}
