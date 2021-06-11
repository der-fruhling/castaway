/*
 * Castaway Built-In Shader `default`
 * Fragment Shader
 */

#version 150 core
// GLSL 1.50 (OpenGL 3.2)

struct PointLight {
    vec3 Position;
    vec3 Color;
};

in vec3 fColor;
in vec3 fNormal;
in vec3 fFragmentPosition;

out vec4 oColor;

uniform float uAmbient = 0.1;
uniform vec3 uAmbientLightColor = vec3(1, 1, 1);
uniform int uPointLightCount = 0;
uniform PointLight uPointLights[32];

void main() {
    vec3 norm = normalize(fNormal);
    
    vec3 lighting = uAmbient * uAmbientLightColor;
    
    for(int i = 0; i < uPointLightCount; i++) {
        PointLight light = uPointLights[i];
        vec3 lightDirection = normalize(light.Position - fFragmentPosition);
        
        float diff = max(dot(norm, lightDirection), 0);
        lighting += diff * light.Color;
    }
    
    vec3 result = lighting * fColor;
    oColor = vec4(result, 1);
}
