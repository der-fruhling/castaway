---
name: Rendering Issue
about: Use for reporting issues with rendering, such as shaders, buffers, and drawing.
title: "[Rendering]: "
labels: rendering
assignees: LiamCoal

---

<!--
Describe the expected behavior here. This is what you expect to see when you run your program.
-->
**Expected Behavior:**
<!-- Add expected behavior here! -->

<!--
Describe the actual behavior here. This is what you actually see when you run your program.
-->
**Actual Behavior:**
<!-- Add actual behavior here! -->

<!--
Castaway is built to support using multiple different APIs. Each API implementation will use different code to render things on the screen. Specify the API you're using here.

For developers, this would be what you call `.Setup()` on, and subsequent `.Get()` calls. ex. `OpenGL.Setup()` & `OpenGL.Get()`

For players, the game might include an option in the menu to change it. If it exists, use that. If the game does not have this option, you may have to do a bit of digging around to find what you're looking for.

This is currently auto-filled to `OpenGL`, because that's the only one available.
-->
**API:** `OpenGL`

<!--
If you're the developer, attaching a code snippet would also be helpful. It should just include the offending portion of the code, don't attach your entire project.
-->
