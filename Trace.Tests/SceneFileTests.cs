using System.IO;
using System.Text;
using Xunit;

namespace Trace.Tests;

public class SceneFileTests
{
    [Fact]
    public void TestInputFile()
    {
        var line = Encoding.ASCII.GetBytes("abc   \nd\nef");
        Stream streamline = new MemoryStream(line);
        var stream = new InputStream(streamline);
        
        Assert.True(stream.Location.LineNum == 1, "Test line 0");
        Assert.True(stream.Location.ColumnNum == 1, "Test col 0");
        
        Assert.True(stream.ReadChar() == "a", "Test read 1");
        Assert.True(stream.Location.LineNum == 1, "Test line 1");
        Assert.True(stream.Location.ColumnNum == 2, "Test col 1");
        
        stream.UnreadChar("A");
        Assert.True(stream.ReadChar() == "A", "Test unread + read 2");
        Assert.True(stream.Location.LineNum == 1, "Test line 2");
        Assert.True(stream.Location.ColumnNum == 2, "Test col 2");
        
        Assert.True(stream.ReadChar() == "b", "Test read 3");
        Assert.True(stream.Location.LineNum == 1, "Test line 3");
        Assert.True(stream.Location.ColumnNum == 3, "Test col 3");
        
        Assert.True(stream.ReadChar() == "c", "Test read 4");
        Assert.True(stream.Location.LineNum == 1, "Test line 4");
        Assert.True(stream.Location.ColumnNum == 4, "Test col 4");
        
        stream.SkipWhitespacesAndComments();
        Assert.True(stream.ReadChar() == "d", "Test read 5");
        Assert.True(stream.Location.LineNum == 2, "Test line 5");
        Assert.True(stream.Location.ColumnNum == 2, "Test col 5");
        
        Assert.True(stream.ReadChar() == "\n", "Test read 6");
        Assert.True(stream.Location.LineNum == 3, "Test line 6");
        Assert.True(stream.Location.ColumnNum == 1, "Test col 6");
        
        Assert.True(stream.ReadChar() == "e", "Test read 7");
        Assert.True(stream.Location.LineNum == 3, "Test line 7");
        Assert.True(stream.Location.ColumnNum == 2, "Test col 7");
        
        Assert.True(stream.ReadChar() == "f", "Test read 8");
        Assert.True(stream.Location.LineNum == 3, "Test line 8");
        Assert.True(stream.Location.ColumnNum == 3, "Test col 8");
        
        Assert.True(stream.ReadChar() == "", "Test end of file");
    }

    public class AssertToken
    {
        public static void AssertIsKeyword(Token token, KeywordEnum keyword)
        {
            Assert.IsType<KeywordToken>(token);
            Assert.True(
                ((KeywordToken) token).Keyword == keyword,
                $"Token {token} is not equal to keyword {keyword}"
            );
        }

        public static void AssertIsIdentifier(Token token, string identifier)
        {
            Assert.IsType<IdentifierToken>(token);
            Assert.True(
                ((IdentifierToken) token).Identifier == identifier,
                $"Expecting identifier {identifier} instead of {token}"
            );
        }

        public static void AssertIsSymbol(Token token, string symbol)
        {
            Assert.IsType<SymbolToken>(token);
            Assert.True(
                ((SymbolToken) token).Symbol == symbol,
                $"Expecting symbol {symbol} instead of {token}"
            );
        }

        public static void AssertIsString(Token token, string s)
        {
            Assert.IsType<StringToken>(token);
            Assert.True(
                ((StringToken) token).Str == s,
                $"Token {token} is not equal to string {s}"
            );
        }

        public static void AssertIsNumber(Token token, float number)
        {
            Assert.IsType<LiteralNumberToken>(token);
            Assert.True(
                Functions.Are_Close(((LiteralNumberToken) token).Value, number),
                $"Token {token} is not equal to number {number}"
            );
        }
    }

    [Fact]
    public void TestLexer()
    {
        var line = Encoding.ASCII.GetBytes(@"
# This is a comment
# This is another comment
new material sky_material(
diffuse(image(""my file.pfm"")),
<5.0, 500.0, 300.0 >
) # Comment at the end of the line");
        //To enable C # keywords to be used as identifiers.
        // The @ character precedes a code element that the compiler
        // must be an identifier rather than a C # keyword 
        Stream streamline = new MemoryStream(line);
        var stream = new InputStream(streamline);

        AssertToken.AssertIsKeyword(stream.ReadToken(), KeywordEnum.New);
        AssertToken.AssertIsKeyword(stream.ReadToken(), KeywordEnum.Material);
        AssertToken.AssertIsIdentifier(stream.ReadToken(), "sky_material");
        AssertToken.AssertIsSymbol(stream.ReadToken(), "(");
        AssertToken.AssertIsKeyword(stream.ReadToken(), KeywordEnum.Diffuse);
        AssertToken.AssertIsSymbol(stream.ReadToken(), "(");
        AssertToken.AssertIsKeyword(stream.ReadToken(), KeywordEnum.Image);
        AssertToken.AssertIsSymbol(stream.ReadToken(), "(");
        AssertToken.AssertIsString(stream.ReadToken(), "my file.pfm");
        AssertToken.AssertIsSymbol(stream.ReadToken(), ")");
        AssertToken.AssertIsSymbol(stream.ReadToken(), ")");
        AssertToken.AssertIsSymbol(stream.ReadToken(), ",");
        AssertToken.AssertIsSymbol(stream.ReadToken(), "<");
        AssertToken.AssertIsNumber(stream.ReadToken(), 5.0f);
        AssertToken.AssertIsSymbol(stream.ReadToken(), ",");
        AssertToken.AssertIsNumber(stream.ReadToken(), 500.0f);
        AssertToken.AssertIsSymbol(stream.ReadToken(), ",");
        AssertToken.AssertIsNumber(stream.ReadToken(), 300.0f);
        AssertToken.AssertIsSymbol(stream.ReadToken(), ">");
        AssertToken.AssertIsSymbol(stream.ReadToken(), ")");
        Assert.IsType<StopToken>(stream.ReadToken());
    }

    [Fact]
    public void TestParser()
    {
        var line = Encoding.ASCII.GetBytes(@"
            float clock(150)
                camera(perspective, rotation_z(30) * translation([-4, 0, 1]), 1.0, 2.0)
                material sky_material(
                 diffuse(uniform(<0, 0, 0>)),
                 uniform(<0.7, 0.5, 1.0>)
                ) 
                
                # here is a comment
                
                material ground_material(
                 diffuse(checkered(<0.3, 0.5, 0.1>, 
                                    <0.1, 0.2, 0.5>, 4)),
                 uniform(<0, 0, 0>)
                )
    
                material sphere_material(
                 specular(uniform(<0.5, 0.5, 0.5>)),
                 uniform(<0, 0, 0>)
                )
    
                plane (sky_material, translation([0.0, 0, 100.000]) * rotation_y(clock))
                plane (ground_material, identity)
                # hi
                sphere(sphere_material, translation([0.0000, 0, 1.00]))
            ");
        
        Stream streamline = new MemoryStream(line);
        var stream = new InputStream(streamline);

        var scene = Scene.ParseScene(inputFile: stream);
        
        // Check that the float variables are ok
        Assert.True(scene.FloatVariables.Count == 1, "Test FloatVariables length");
        Assert.True(scene.FloatVariables.ContainsKey("clock"), "Test FloatVariables contains");
        Assert.True(Functions.Are_Close(scene.FloatVariables["clock"],150.0f), "Test FloatVariables value");
        
        // Check that the materials are ok
        Assert.True(scene.Materials.Count == 3, "Test Materials length");
        Assert.True(scene.Materials.ContainsKey("sphere_material"), "Test Materials contains 1");
        Assert.True(scene.Materials.ContainsKey("sky_material"), "Test Materials contains 2");
        Assert.True(scene.Materials.ContainsKey("ground_material"), "Test Materials contains 3");

        var sphereMaterial = scene.Materials["sphere_material"];
        var skyMaterial = scene.Materials["sky_material"];
        var groundMaterial = scene.Materials["ground_material"];
        
        Assert.IsType<DiffuseBrdf>(skyMaterial.Brdf);
        Assert.IsType<UniformPigment>(skyMaterial.Brdf.Pg);
        Assert.True(((UniformPigment) skyMaterial.Brdf.Pg).C.Is_Close(new Color()));
        
        Assert.IsType<DiffuseBrdf>(groundMaterial.Brdf);
        Assert.IsType<CheckeredPigment>(groundMaterial.Brdf.Pg);
        Assert.True(((CheckeredPigment) groundMaterial.Brdf.Pg).C1.Is_Close(new Color(0.3f, 0.5f, 0.1f)));
        Assert.True(((CheckeredPigment) groundMaterial.Brdf.Pg).C2.Is_Close(new Color(0.1f, 0.2f, 0.5f)));
        Assert.True(((CheckeredPigment) groundMaterial.Brdf.Pg).NumOfSteps == 4);
        
        Assert.IsType<SpecularBrdf>(sphereMaterial.Brdf);
        Assert.IsType<UniformPigment>(sphereMaterial.Brdf.Pg);
        Assert.True(((UniformPigment) sphereMaterial.Brdf.Pg).C.Is_Close(new Color(0.5f, 0.5f, 0.5f)));

        Assert.IsType<UniformPigment>(skyMaterial.EmittedRadiance);
        Assert.True(((UniformPigment) skyMaterial.EmittedRadiance).C.Is_Close(new Color(0.7f, 0.5f, 1.0f)));
        Assert.IsType<UniformPigment>(groundMaterial.EmittedRadiance);
        Assert.True(((UniformPigment) groundMaterial.EmittedRadiance).C.Is_Close(new Color()));
        Assert.IsType<UniformPigment>(sphereMaterial.EmittedRadiance);
        Assert.True(((UniformPigment) sphereMaterial.EmittedRadiance).C.Is_Close(new Color()));
        
        // Check that the shapes are ok
        Assert.True(scene.Wd.World1.Count == 3, "Test World length");
        Assert.IsType<Plane>(scene.Wd.World1[0]);
        Assert.True(scene.Wd.World1[0].Tr.Is_Close(Transformation.Translation(new Vec(0.0f, 0.0f, 100.0f)) * Transformation.Rotation_Y(150.0f)));
        Assert.IsType<Plane>(scene.Wd.World1[1]);
        Assert.True(scene.Wd.World1[1].Tr.Is_Close(Transformation.Identity()));
        Assert.IsType<Sphere>(scene.Wd.World1[2]);
        Assert.True(scene.Wd.World1[2].Tr.Is_Close(Transformation.Translation(new Vec(0.0f, 0.0f, 1.0f))));
        
        // Check that the camera is ok
        Assert.IsType<PerspectiveCamera>(scene.Camera);
        Assert.True(((PerspectiveCamera) scene.Camera!).T.Is_Close(Transformation.Rotation_Z(30.0f) * Transformation.Translation(new Vec(-4.0f, 0.0f, 1.0f))));
        Assert.True(Functions.Are_Close(((PerspectiveCamera) scene.Camera!).AspectRatio, 1.0f));
        Assert.True(Functions.Are_Close(((PerspectiveCamera) scene.Camera!).Distance, 2.0f));
    }

    [Fact]
    public void TestParserUndefinedMaterial()
    {
        // Check that unknown materials raise a GrammarError
        var line = Encoding.ASCII.GetBytes(@"plane(this_material_does_not_exist, identity)");
        
        Stream streamline = new MemoryStream(line);
        var stream = new InputStream(streamline);
        
        var ex = Assert.Throws<GrammarErrorException>(() => Scene.ParseScene(inputFile: stream));
        Assert.Contains("unknown material this_material_does_not_exist", ex.Message);
    }

    [Fact]
    public void TestParserDoubleCamera()
    {
        // Check that defining two cameras in the same file raises a GrammarError
        var line = Encoding.ASCII.GetBytes(@"
            camera(perspective, rotation_z(30) * translation([-4, 0, 1]), 1.0, 1.0)
            camera(orthogonal, identity, 1.0, 1.0)
        ");
        
        Stream streamline = new MemoryStream(line);
        var stream = new InputStream(streamline);
        
        var ex = Assert.Throws<GrammarErrorException>(() => Scene.ParseScene(inputFile: stream));
        Assert.Contains("You cannot define more than one camera", ex.Message);
    }

    [Fact]
    public void TestParserCsg()
    {
        var line = Encoding.ASCII.GetBytes(@"
                camera(perspective, rotation_z(30) * translation([-4, 0, 1]), 1.0, 2.0)
                material sky_material(
                 diffuse(uniform(<0, 0, 0>)),
                 uniform(<0.7, 0.5, 1.0>)
                ) 
                # here is a comment
                union(
                    intersection(sphere(sky_material, translation([0, 0, 1.00])), cylinder(sky_material, identity), identity),
                    difference(box([-1, -1, -1], [1, 1, 1], sky_material, translation([0, 0, 1.00])), disk(sky_material, identity), identity),
                    identity)
            ");
        
        Stream streamline = new MemoryStream(line);
        var stream = new InputStream(streamline);

        var scene = Scene.ParseScene(inputFile: stream);

        // check world is ok
        Assert.True(scene.Wd.World1.Count == 1, "Test World length");
        Assert.IsType<CsgUnion>(scene.Wd.World1[0]);
        Assert.IsType<CsgIntersection>(((CsgUnion) scene.Wd.World1[0]).S1);
        Assert.IsType<CsgDifference>(((CsgUnion) scene.Wd.World1[0]).S2);
        Assert.IsType<Sphere>(((CsgIntersection)((CsgUnion) scene.Wd.World1[0]).S1).S1);
        Assert.IsType<Cylinder>(((CsgIntersection)((CsgUnion) scene.Wd.World1[0]).S1).S2);
        Assert.IsType<Box>(((CsgDifference)((CsgUnion) scene.Wd.World1[0]).S2).S1);
        Assert.True(((Box) (((CsgDifference)((CsgUnion) scene.Wd.World1[0]).S2).S1)).Bounds[0].Is_Close(new Point(1.0f, 1.0f, 1.0f)));
        Assert.True(((Box) (((CsgDifference)((CsgUnion) scene.Wd.World1[0]).S2).S1)).Bounds[1].Is_Close(new Point(-1.0f, -1.0f, -1.0f)));
        Assert.IsType<Disk>(((CsgDifference)((CsgUnion) scene.Wd.World1[0]).S2).S2);
    }
}