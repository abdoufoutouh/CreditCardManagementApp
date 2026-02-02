<template>
  <div class="sidebar">
    <div class="sidebar-header">
      <div class="logo">
        <h2>CardManager</h2>
      </div>
    </div>
    
    <nav class="sidebar-nav">
      <div class="nav-item" @click="$router.push('/dashboard')">
        <span class="nav-icon">📊</span>
        <span class="nav-text">Dashboard</span>
      </div>
      <div class="nav-item" @click="$router.push('/creditcards/create')">
        <span class="nav-icon">➕</span>
        <span class="nav-text">Create Card</span>
      </div>
      <div class="nav-item">
        <span class="nav-icon">💳</span>
        <span class="nav-text">My Cards</span>
      </div>
      <div class="nav-item">
        <span class="nav-icon">📋</span>
        <span class="nav-text">Transactions</span>
      </div>
      <div class="nav-item">
        <span class="nav-icon">⚙️</span>
        <span class="nav-text">Settings</span>
      </div>
    </nav>
  </div>
</template>

<style scoped>
.sidebar {
  width: 260px;
  height: 100vh;
  background: linear-gradient(135deg, #1e3c72 0%, #2a5298 100%);
  position: fixed;
  left: 0;
  top: 0;
  display: flex;
  flex-direction: column;
  box-shadow: 2px 0 10px rgba(0, 0, 0, 0.1);
}

.sidebar-header {
  padding: 30px 25px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.logo h2 {
  color: white;
  margin: 0;
  font-size: 24px;
  font-weight: 700;
  letter-spacing: -0.5px;
}

.sidebar-nav {
  flex: 1;
  padding: 20px 0;
}

.nav-item {
  display: flex;
  align-items: center;
  padding: 15px 25px;
  color: rgba(255, 255, 255, 0.7);
  cursor: pointer;
  transition: all 0.3s ease;
  border-left: 3px solid transparent;
}

.nav-item:hover {
  background: rgba(255, 255, 255, 0.1);
  color: white;
}

.nav-item.active {
  background: rgba(255, 255, 255, 0.15);
  color: white;
  border-left-color: #4CAF50;
}

.nav-icon {
  font-size: 18px;
  margin-right: 15px;
  width: 20px;
  text-align: center;
}

.nav-text {
  font-size: 15px;
  font-weight: 500;
}
</style>
