using AspNotes.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using Moq;

namespace AspNotes.Tests.Common;

public class BearerSecuritySchemeTransformerTests
{
    private readonly Mock<IAuthenticationSchemeProvider> authenticationSchemeProviderMock;
    private readonly BearerSecuritySchemeTransformer transformer;

    public BearerSecuritySchemeTransformerTests()
    {
        authenticationSchemeProviderMock = new Mock<IAuthenticationSchemeProvider>();
        transformer = new BearerSecuritySchemeTransformer(authenticationSchemeProviderMock.Object);
    }

    [Fact]
    public async Task TransformAsync_ShouldAddBearerScheme_WhenBearerSchemeIsPresent()
    {
        // Arrange
        var schemes = new List<AuthenticationScheme>
        {
            new("Bearer", null, typeof(IAuthenticationHandler))
        };

        authenticationSchemeProviderMock.Setup(x => x.GetAllSchemesAsync())
            .ReturnsAsync(schemes);

        var document = new OpenApiDocument
        {
            Paths = new OpenApiPaths
            {
                ["/test"] = new OpenApiPathItem
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Get] = new OpenApiOperation()
                    }
                }
            }
        };

        // Act
        await transformer.TransformAsync(document, null, CancellationToken.None);

        // Assert
        Assert.NotNull(document.Components);
        Assert.True(document.Components.SecuritySchemes.ContainsKey("Bearer"));
        Assert.All(document.Paths.Values.SelectMany(path => path.Operations.Values),
            operation => Assert.Contains(operation.Security, securityRequirement =>
                securityRequirement.ContainsKey(new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
                })));
    }

    [Fact]
    public async Task TransformAsync_ShouldNotAddBearerScheme_WhenBearerSchemeIsAbsent()
    {
        // Arrange
        authenticationSchemeProviderMock.Setup(x => x.GetAllSchemesAsync())
            .ReturnsAsync([]);

        var document = new OpenApiDocument
        {
            Paths = new OpenApiPaths
            {
                ["/test"] = new OpenApiPathItem
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Get] = new OpenApiOperation()
                    }
                }
            }
        };

        // Act
        await transformer.TransformAsync(document, null, CancellationToken.None);

        // Assert
        Assert.Null(document.Components?.SecuritySchemes);
        Assert.All(document.Paths.Values.SelectMany(path => path.Operations.Values),
            operation => Assert.DoesNotContain(operation.Security, securityRequirement =>
                securityRequirement.ContainsKey(new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
                })));
    }
}
