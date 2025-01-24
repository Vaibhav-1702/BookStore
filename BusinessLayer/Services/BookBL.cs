using BusinessLayer.Interface;
using DataLayer.Interface;
using Model.DTO;
using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class BookBL : IBookBL
    {
        private readonly IBookDL _bookDL;

        public BookBL(IBookDL bookDL)
        {
           _bookDL = bookDL;
                
        }
        public async Task<ResponseModel<Book>> AddBook(BookDto bookDto)
        {
            return await _bookDL.AddBook(bookDto); 
        }

        public async Task<ResponseModel<bool>> DeleteBookById(int bookId)
        {
            return await _bookDL.DeleteBookById(bookId);
        }

        public async Task<ResponseModel<IEnumerable<Book>>> GetAllBooks()
        {
            return await  _bookDL.GetAllBooks();
        }

        public async Task<ResponseModel<Book>> GetBookById(int bookId)
        {
            return await _bookDL.GetBookById(bookId);
        }

        public async Task<ResponseModel<Book>> UpdateBook(int bookId, UpdateBookDto updateBookDto)
        {
           return  await _bookDL.UpdateBook(bookId, updateBookDto);
        }
    }
}
