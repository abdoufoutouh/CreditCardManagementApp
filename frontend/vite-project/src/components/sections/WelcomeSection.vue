<template>
  <div class="welcome-section">
    <div class="welcome-content">
      <h1 class="welcome-title">
        Welcome back 👋
      </h1>
      <p class="welcome-subtitle">
        You are successfully connected.
      </p>
    </div>
  </div>
</template>

<style scoped>
.welcome-section {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 20px;
  padding: 40px;
  margin-bottom: 30px;
  position: relative;
  overflow: hidden;
  box-shadow: 0 8px 24px rgba(102, 126, 234, 0.3);
}

.welcome-section::before {
  content: '';
  position: absolute;
  top: -50%;
  right: -10%;
  width: 300px;
  height: 300px;
  background: radial-gradient(circle, rgba(255, 255, 255, 0.1) 0%, transparent 70%);
  border-radius: 50%;
}

.welcome-section::after {
  content: '';
  position: absolute;
  bottom: -30%;
  left: -5%;
  width: 200px;
  height: 200px;
  background: radial-gradient(circle, rgba(255, 255, 255, 0.08) 0%, transparent 70%);
  border-radius: 50%;
}

.welcome-content {
  position: relative;
  z-index: 1;
}

.welcome-title {
  margin: 0 0 15px 0;
  font-size: 36px;
  font-weight: 700;
  color: white;
  line-height: 1.2;
}

.welcome-subtitle {
  margin: 0;
  font-size: 18px;
  color: rgba(255, 255, 255, 0.9);
  font-weight: 400;
}
</style>
