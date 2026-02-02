<template>
  <div class="dashboard-layout">
    <Sidebar />
    <Topbar :title="title" :username="username" />
    <main class="main-content">
      <div class="content-wrapper">
        <slot />
      </div>
    </main>
  </div>
</template>

<script setup>
import Sidebar from './Sidebar.vue';
import Topbar from './Topbar.vue';

defineProps({
  title: {
    type: String,
    default: 'Dashboard'
  },
  username: {
    type: String,
    default: 'John Doe'
  }
});
</script>

<style scoped>
.dashboard-layout {
  min-height: 100vh;
  background: #f5f7fa;
}

.main-content {
  margin-left: 260px;
  margin-top: 70px;
  min-height: calc(100vh - 70px);
}

.content-wrapper {
  padding: 30px;
  max-width: 1400px;
  margin: 0 auto;
}
</style>
