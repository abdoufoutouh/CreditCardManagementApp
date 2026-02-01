<template>
  <div class="stats-section">
    <div class="stats-grid">
      <StatCard
        v-for="stat in stats"
        :key="stat.id"
        :title="stat.title"
        :value="stat.value"
        :icon="stat.icon"
        :icon-color="stat.iconColor"
      />
    </div>
  </div>
</template>

<script setup>
import StatCard from '../cards/StatCard.vue';

defineProps({
  stats: {
    type: Array,
    required: true
  }
});
</script>

<style scoped>
.stats-section {
  margin-bottom: 30px;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
}
</style>
