#version 150 core

in vec4 color;
in vec3 normal;
in vec3 fragPos;
in vec2 texCoords;

out vec4 outColor;

uniform vec3 lightPos;
uniform vec3 viewPos;

struct Material
{
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float specularExp;
};

uniform Material material;
uniform sampler2D tex;
uniform bool shouldTexture;

void main()
{
    vec3 norm = normalize(normal);
    vec3 cullViewDir = normalize(-viewPos - fragPos);
    if(dot(cullViewDir, norm) < 0) discard;
    
    float ambientStrength = 0.1;
    float specularStrength = 0.5;
    
    vec3 ambient = ambientStrength * material.ambient;
    
    vec3 lightDir = normalize(lightPos - fragPos);
    float diff = max(dot(norm, lightDir), 0);
    vec3 diffuse = diff * material.diffuse;
    
    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0), material.specularExp);
    vec3 specular = specularStrength * spec * material.specular;
    
    outColor = vec4(ambient + diffuse + specular, 1) *
        ((shouldTexture ? texture(tex, texCoords) : vec4(1, 1, 1, 1)) * color);
}
