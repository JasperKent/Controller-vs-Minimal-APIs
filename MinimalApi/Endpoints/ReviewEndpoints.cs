using DataAccess;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
//namespace MinimalApi.Endpoints;

//public static class ReviewEndpoints
//{
//    public static void MapBookReviewEndpoints (this IEndpointRouteBuilder routes)
//    {
//        var group = routes.MapGroup("/api/BookReview").WithTags(nameof(BookReview));

//        group.MapGet("/", () =>
//        {
//            return new [] { new BookReview() };
//        })
//        .WithName("GetAllBookReviews")
//        .WithOpenApi();

//        group.MapGet("/{id}", (int id) =>
//        {
//            //return new BookReview { ID = id };
//        })
//        .WithName("GetBookReviewById")
//        .WithOpenApi();

//        group.MapPut("/{id}", (int id, BookReview input) =>
//        {
//            return TypedResults.NoContent();
//        })
//        .WithName("UpdateBookReview")
//        .WithOpenApi();

//        group.MapPost("/", (BookReview model) =>
//        {
//            //return TypedResults.Created($"/api/BookReviews/{model.ID}", model);
//        })
//        .WithName("CreateBookReview")
//        .WithOpenApi();

//        group.MapDelete("/{id}", (int id) =>
//        {
//            //return TypedResults.Ok(new BookReview { ID = id });
//        })
//        .WithName("DeleteBookReview")
//        .WithOpenApi();
//    }
//}
