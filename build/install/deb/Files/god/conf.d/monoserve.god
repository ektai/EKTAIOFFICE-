module God
  module Conditions
    class SocketConnectedWithinTimeout < SocketResponding
      def test
        socket = nil
        self.info = []
        begin
          Timeout.timeout(5) do
            socket = UNIXSocket.new(self.path)
          end
        rescue Timeout::Error
          self.info = 'Socket connection timeout'
          return true
        rescue Exception => ex
          self.info = "Failed connected to socket with exception: #{ex}"
          socket.close if socket.is_a?(UNIXSocket)
          return true
        end
        socket.close
        self.info = 'Unix socket is responding'
        return false
      end
    end
  end
end

God.watch do |w|
  w.name = "monoserve"
  w.group = "EKTAIOFFICE"
  w.grace = 15.seconds
  w.start = "/etc/init.d/monoserve start"
  w.stop = "/etc/init.d/monoserve stop"
  w.restart = "/etc/init.d/monoserve restart"
  w.pid_file = "/tmp/monoserve"
  w.unix_socket = "/var/run/EKTAIOFFICE/EKTAIOFFICE.socket"

  w.start_if do |start|
    start.condition(:process_running) do |c|
      c.interval = 10.seconds
      c.running = false
    end
  end

  w.restart_if do |restart|
    restart.condition(:socket_connected_within_timeout) do |c|
      c.family = 'unix'
      c.path = '/var/run/EKTAIOFFICE/EKTAIOFFICE.socket'
      c.times = 5
      c.interval = 5.seconds
    end
    restart.condition(:http_response_code) do |c|
      c.host = 'localhost'
      c.path = '/api/2.0/capabilities.json'
      c.code_is_not = 200
      c.times = 5
      c.interval = 5.seconds
    end
    restart.condition(:cpu_usage) do |c|
      c.above = 90.percent
      c.times = 5
      c.interval = 3.minutes
    end
  end
end
