using eCommerce.SharedLibrary.Responses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Application.DTOs;
using ProductAPI.Application.Interfaces;
using ProductAPI.Domain.Entities;
using ProductAPI.Presentation.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ProductAPI.Controller
{
    public class ProductControllerTest
    {
        private readonly IProduct productInterface;
        private readonly ProductController productController;
        public ProductControllerTest()
        {
            //set up dependencies
            productInterface = A.Fake<IProduct>();
            // set up system under test
            productController = new ProductController(productInterface);
        }
        //Get All Products 
        [Fact]
        public async Task GetProductWhenProductExists_ReturnOkResponse()
        {
            //Arrange
            var products = new List<Product>()
            {
                 new Product { Id = 1,Name = "Product 1", Quantity = 1 ,Price = 100},
                 new Product { Id = 2,Name = "Product 2", Quantity = 2 ,Price = 200},
                 new Product { Id = 3,Name = "Product 3", Quantity = 3 ,Price = 300}
            };
            //set up fake response for GetAllAsyncs method
            A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

            //act
            var result = await productController.GetProducts();

            //Assert
            var OkResult = result.Result as OkObjectResult;
            OkResult.Should().NotBeNull();
            OkResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var returnProducts = OkResult.Value as IEnumerable<ProductDTO>;
            returnProducts.Should().NotBeNull();
            returnProducts.Should().HaveCount(3);

            var productList = returnProducts!.ToList();
            productList[0].Id.Should().Be(1);
            productList[1].Id.Should().Be(2);
            productList[2].Id.Should().Be(3);
        }
        [Fact]
        public async Task GetProductWhenProductNotExists_ReturnNotFoundResponse()
        {
            var products = new List<Product>();

            // set up fake response for getallasync();
            A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

            //act
            var result = await productController.GetProducts();

            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);

            var message = notFoundResult.Value as string;
            message.Should().Be("No products detected in the db");
        }

        [Fact]
        public async Task CreateProductInvalidModel_ReturnBadResponse()
        {
            var productDTO = new ProductDTO(1, "Product 1", 30, 100);
            productController.ModelState.AddModelError("Name", "Required");

            var result = await productController.CreateProduct(productDTO);

            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task CreateProduct_ReturnOkResponse()
        {
            //arrange
            var productDTO = new ProductDTO(1, "Product 1", 30, 100);
            var response = new Response(true, "Created");

            //act
            A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);
            var result = await productController.CreateProduct(productDTO);

            //Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = okResult.Value as Response;
            responseResult.Should().NotBeNull();
            responseResult.Message.Should().Be("Created");
            responseResult.Flag.Should().BeTrue();

        }

        [Fact]

        public async Task CreateProductFail_ReturnBadRequestResponse()
        {
            var productDTO = new ProductDTO(1, "Product 1", 30, 100);
            var response = new Response(false, "Failed");

            //act
            A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);//set up to get fake response from function
            var result = await productController.CreateProduct(productDTO);

            //Assert
            var okResult = result.Result as BadRequestObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = okResult.Value as Response;
            responseResult.Should().NotBeNull();
            responseResult.Message.Should().Be("Failed");
            responseResult.Flag.Should().BeFalse();
        }

        [Fact]

        public async Task UpdateProductSuccess_ReturnOKResponse()
        {
            var productDTO = new ProductDTO(1, "Product 1", 30, 100);
            var response = new Response(true, "Updated");

            //act
            A.CallTo(() => productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);//set up to get fake response from function
            var result = await productController.UpdateProduct(productDTO);

            //Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = okResult.Value as Response;
            responseResult.Should().NotBeNull();
            responseResult.Message.Should().Be("Updated");
            responseResult.Flag.Should().BeTrue();
        }


        [Fact]

        public async Task UpdateProductFail_ReturnBadRequestResponse()
        {
            var productDTO = new ProductDTO(1, "Product 1", 30, 100);
            var response = new Response(false, "Failed");

            //act
            A.CallTo(() => productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);
            var result = await productController.UpdateProduct(productDTO);

            //Assert
            var okResult = result.Result as BadRequestObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = okResult.Value as Response;
            responseResult.Should().NotBeNull();
            responseResult.Message.Should().Be("Failed");
            responseResult.Flag.Should().BeFalse();
        }

        [Fact]

        public async Task DeleteProductSuccess_ReturnOkResponse()
        {
            var productDTO = new ProductDTO(1, "Product 1", 30, 100);
            var response = new Response(true, "Delete Success");

            //act
            A.CallTo(() => productInterface.DeleteAsync(A<Product>.Ignored)).Returns(response);
            var result = await productController.DeleteProduct(productDTO);

            //Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = okResult.Value as Response;
            responseResult.Should().NotBeNull();
            responseResult.Message.Should().Be("Delete Success");
            responseResult.Flag.Should().BeTrue();
        }

        [Fact]

        public async Task DeleteProductFail_ReturnBadRequestResponse()
        {
            var productDTO = new ProductDTO(1, "Product 1", 30, 100);
            var response = new Response(false, "Delete Failed");

            //set up 
            A.CallTo(() => productInterface.DeleteAsync(A<Product>.Ignored)).Returns(response);

            //act
            var result = await productController.DeleteProduct(productDTO);

            //Assert
            var okResult = result.Result as BadRequestObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = okResult.Value as Response;
            responseResult.Should().NotBeNull();
            responseResult.Message.Should().Be("Delete Failed");
            responseResult.Flag.Should().BeFalse();
        }
    }
}
