/*
 * Phong lighting model.
 * Combines three types of lighting: Ambient, Diffuse, and Specular.
 *
 * Fragment shader.
 */

#version 150 core

// Structure to contain some info about what materials should look like.
struct Material
{
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float specularExp;
};

// Structure to contain some info about directional lights.
struct DirectionalLight
{
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct PointLight
{
    vec3 position;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    
    float constant;
    float linear;
    float quadratic;
};

struct Spotlight
{
    vec3 position;
    vec3 direction;
    float cutOff;
    float outerCutOff;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

// Passed from vertex shader.
in vec4 color;
in vec3 normal;
in vec3 fragPos;
in vec2 texCoords;

// Output color.
out vec4 outColor;

// Filled with knowledge.
uniform Material material;

#define DIR_MAX 8
#define PNT_MAX 64
#define SPT_MAX 64

uniform DirectionalLight dirLights[DIR_MAX];
uniform PointLight pointLights[PNT_MAX];
uniform Spotlight spotlights[SPT_MAX];
uniform int dirCount;
uniform int pointCount;
uniform int spotCount;

// Camera position.
uniform vec3 viewPos;

// Currently bound texture.
uniform sampler2D tex;

// Used by `Texture` to disable textures when needed.
uniform bool shouldTexture;

// Some constants.
const float specularStrength = 0.5;
const float ambientStrength = 0.1;
const vec4 defaultColor = vec4(1, 1, 1, 1);

// Calculate Diffuse lighting.
vec3 calculateDiffuse(vec3 lightPos, vec3 norm, vec3 lightDir)
{
    float diff = max(dot(norm, lightDir), 0);
    vec3 diffuse = diff * material.diffuse;
    
    return diffuse;
}

// Calculate Specular lighting.
vec3 calculateSpecular(vec3 lightPos, vec3 norm, vec3 lightDir)
{
    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0), material.specularExp);
    vec3 specular = specularStrength * spec * material.specular;
    
    return specular;
}

// Combine Diffuse and Specular lighting.
vec3 calculateLighting(vec3 lightPos)
{
    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(lightPos - fragPos);

    return calculateDiffuse(lightPos, norm, lightDir) + calculateSpecular(lightPos, norm, lightDir);
}

// Calculate directional lights.
vec3 CalcDirectional(DirectionalLight dir)
{
    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(-dir.direction);
    vec3 lightPos = vec3(0, 0, 0);

    return calculateDiffuse(lightPos, norm, lightDir) + calculateSpecular(lightPos, norm, lightDir);
}

// Calculate point lights.
vec3 CalcPoint(PointLight point)
{
    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(point.position - fragPos);
    
    float distance = length(point.position - fragPos);
    float attenuation = 1.0 / (point.constant + point.linear * distance + point.quadratic * pow(distance, 2));

    return (calculateDiffuse(point.position, norm, lightDir) * attenuation) + 
           (calculateSpecular(point.position, norm, lightDir) * attenuation);
}

vec3 CalcSpotlight(Spotlight spot)
{
    vec3 lightDir = normalize(spot.position - fragPos);
    float theta = dot(lightDir, normalize(-spot.direction));
    float epsilon = spot.cutOff - spot.outerCutOff;
    float intensity = clamp((theta - spot.outerCutOff) / epsilon, 0.0, 1.0);
    vec3 norm = normalize(normal);

    return (calculateDiffuse(spot.position, norm, lightDir) * intensity) +
           (calculateSpecular(spot.position, norm, lightDir) * intensity);
}

void main()
{
    // Do not render unseen pixels.
    if(dot(normalize(viewPos - fragPos), normalize(normal)) < 0) discard;
    
    // Calculate all diffuse and specular lightings.
    vec3 lighting = vec3(0, 0, 0);
    
    for(int i = 0; i < min(dirCount, DIR_MAX); i++)
        lighting += CalcDirectional(dirLights[i]);
    for(int i = 0; i < min(pointCount, PNT_MAX); i++)
        lighting += CalcPoint(pointLights[i]);
    for(int i = 0; i < min(spotCount, SPT_MAX); i++)
        lighting += CalcSpotlight(spotlights[i]);

    // Calculate ambient lighting.
    vec3 ambient = ambientStrength * material.ambient;
    
    // Combine all the lightings!
    outColor = vec4(ambient + lighting, 1) * ((shouldTexture ? texture(tex, texCoords) : defaultColor) * color);
}
