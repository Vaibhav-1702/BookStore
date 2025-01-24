using DataLayer.Context;
using DataLayer.Interface;
using Microsoft.EntityFrameworkCore;
using Model.DTO;
using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repository
{
    public class BookDL : IBookDL
    {
        private readonly BookStoreContext _context;

        public BookDL(BookStoreContext context)
        {
            _context = context; 
        }

        public async Task<ResponseModel<Book>> AddBook(BookDto bookDto)
        {
            try
            {
                var book = new Book
                {
                    Title = bookDto.Title,
                    Author = bookDto.Author,
                    Description = bookDto.Description,
                    Price = bookDto.Price,
                    Stock = bookDto.Stock,
                    PublicationDate = bookDto.PublicationDate
                };

                await _context.Books.AddAsync(book);
                await _context.SaveChangesAsync();

                return new ResponseModel<Book>
                {
                    Data = book,
                    Message = "Book added successfully",
                    StatusCode = (int)HttpStatusCode.Created,
                    Success = true
                };
            }
            catch (Exception)
            {
                return new ResponseModel<Book>
                {
                    Data = null,
                    Message = "An error occurred while adding the book",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        public async Task<ResponseModel<IEnumerable<Book>>> GetAllBooks()
        {
            try
            {
                var books = await _context.Books.ToListAsync();

                return new ResponseModel<IEnumerable<Book>>
                {
                    Data = books,
                    Message = "Books retrieved successfully",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception)
            {
                return new ResponseModel<IEnumerable<Book>>
                {
                    Data = null,
                    Message = "An error occurred while retrieving books",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        public async Task<ResponseModel<Book>> GetBookById(int bookId)
        {
            try
            {
                var book = await _context.Books.FindAsync(bookId);

                if (book == null)
                {
                    return new ResponseModel<Book>
                    {
                        Data = null,
                        Message = "Book not found",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }

                return new ResponseModel<Book>
                {
                    Data = book,
                    Message = "Book retrieved successfully",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception)
            {
                return new ResponseModel<Book>
                {
                    Data = null,
                    Message = "An error occurred while retrieving the book",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        public async Task<ResponseModel<bool>> DeleteBookById(int bookId)
        {
            try
            {
                var book = await _context.Books.FindAsync(bookId);

                if (book == null)
                {
                    return new ResponseModel<bool>
                    {
                        Data = false,
                        Message = "Book not found",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                return new ResponseModel<bool>
                {
                    Data = true,
                    Message = "Book deleted successfully",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception)
            {
                return new ResponseModel<bool>
                {
                    Data = false,
                    Message = "An error occurred while deleting the book",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        public async Task<ResponseModel<Book>> UpdateBook(int bookId, UpdateBookDto updateBookDto)
        {
            try
            {
                var book = await _context.Books.FindAsync(bookId);

                if (book == null)
                {
                    return new ResponseModel<Book>
                    {
                        Data = null,
                        Message = "Book not found",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }

                // Update book details
                book.Title = updateBookDto.Title;
                book.Author = updateBookDto.Author;
                book.Description = updateBookDto.Description;
                book.Price = updateBookDto.Price;
                book.Stock = updateBookDto.Stock;
                book.PublicationDate = updateBookDto.PublicationDate;

                _context.Books.Update(book);
                await _context.SaveChangesAsync();

                return new ResponseModel<Book>
                {
                    Data = book,
                    Message = "Book updated successfully",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception)
            {
                return new ResponseModel<Book>
                {
                    Data = null,
                    Message = "An error occurred while updating the book",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

    }
}
