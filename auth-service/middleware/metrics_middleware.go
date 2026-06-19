package middleware

import (
	"strconv"
	"time"

	"github.com/gin-gonic/gin"
	"github.com/prometheus/client_golang/prometheus"
	"github.com/prometheus/client_golang/prometheus/promauto"
)

var (
	httpRequestsTotal = promauto.NewCounterVec(
		prometheus.CounterOpts{
			Name: "http_requests_total",
			Help: "Total de requisições HTTP",
		},
		[]string{"path", "method", "status"},
	)

	httpDuration = promauto.NewHistogramVec(
		prometheus.HistogramOpts{
			Name:    "http_request_duration_seconds",
			Help:    "Duração das requisições HTTP",
			Buckets: prometheus.DefBuckets,
		},
		[]string{"path"},
	)
)

func PrometheusMiddleware() gin.HandlerFunc {
	return func(context *gin.Context) {
		start := time.Now()

		context.Next()

		duration := time.Since(start).Seconds()
		path := context.FullPath()
		if path == "" {
			path = "unmatched"
		}

		status := strconv.Itoa(context.Writer.Status())

		httpRequestsTotal.WithLabelValues(path, context.Request.Method, status).Inc()
		httpDuration.WithLabelValues(path).Observe(duration)
	}
}
