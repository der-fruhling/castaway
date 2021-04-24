/*
 * Phong lighting model.
 * Combines three types of lighting: Ambient, Diffuse, and Specular.
 *
 * Vertex shader.
 */

#version 150 core

// Vertex attributes. Set up in config.csh.
in vec3 inPosition;
in vec4 inColor;
in vec3 inNormal;
in vec2 inTexCoords;

// Outputs to fragment shader.
out vec4 color;
out vec3 normal;
out vec3 fragPos;
out vec2 texCoords;

// Transformation matrices.
uniform mat4 model;
uniform mat4 view;
uniform mat4 proj;

// Terrible optimisation. TODO
mat4 lastModel = mat4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
mat4 tiModel = mat4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

void main()
{
    // More terrible optimisation.
    if(model != lastModel) {
        tiModel = transpose(inverse(model));
        lastModel = model;
    }
    
    // gl_Position is the end position of the vertex.
    gl_Position = proj * view * model * vec4(inPosition, 1);
    
    // Set fragment shader values.
    color = inColor;
    texCoords = inTexCoords;
    
    // Tells the fragment shader where each pixel is.
    fragPos = vec3(model * vec4(inPosition, 1));
    
    // Transformed like this to prevent weirdness.
    normal = mat3(tiModel) * inNormal;
}
