#version 150 core

in vec4 color;
in vec3 normal;
in vec3 fragPos;

out vec4 outColor;

uniform vec3 lightPos;
uniform vec3 viewPos;

void main()
{
    float ambientStrength = 0.1;
    float specularStrength = 0.5;
    
    vec3 lightColor = vec3(1, 1, 1);
    vec3 ambient = ambientStrength * lightColor;
    
    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(lightPos - fragPos);
    float diff = max(dot(norm, lightDir), 0);
    vec3 diffuse = diff * lightColor;
    
    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0), 32);
    vec3 specular = specularStrength * spec * lightColor;
    
    outColor = vec4(ambient + diffuse + specular, 1) * color;
}
